using Microsoft.AspNetCore.DataProtection;
using MongoDB.Driver;

namespace SofiPOS.FnB.Demo.Services;

public sealed class ConnectionService(DemoStore store, IDataProtectionProvider protection)
{
    private readonly IDataProtector _protector = protection.CreateProtector("SofiPOS.FnB.Demo.Connection.v1");

    public Task<ConnectionRecord?> GetAsync(CancellationToken ct = default)
        => store.Connections.Find(x => x.Id == "current").FirstOrDefaultAsync(ct)!;

    public async Task<ConnectionRecord> RequiredAsync(CancellationToken ct = default)
        => await GetAsync(ct) ?? throw new InvalidOperationException("Chưa cấu hình kết nối. Mở trang Kết nối và lưu Partner Key trước.");

    public string ApiKey(ConnectionRecord record) => _protector.Unprotect(record.ProtectedApiKey);
    public string WebhookSecret(ConnectionRecord record) => string.IsNullOrEmpty(record.ProtectedWebhookSecret) ? "" : _protector.Unprotect(record.ProtectedWebhookSecret);

    public static object View(ConnectionRecord? record) => new
    {
        configured = record != null,
        connectionId = record?.ConnectionId,
        name = record?.Name ?? "Cửa hàng thử nghiệm",
        baseUrl = record?.BaseUrl ?? "",
        hasApiKey = !string.IsNullOrEmpty(record?.ProtectedApiKey),
        hasWebhookSecret = !string.IsNullOrEmpty(record?.ProtectedWebhookSecret),
        defaultBranchId = record?.DefaultBranchId,
        defaultSaleChannelId = record?.DefaultSaleChannelId ?? "TaiCho",
        updatedAt = record?.UpdatedAt
    };

    public async Task<ConnectionRecord> SaveAsync(ConnectionInput input, CancellationToken ct)
    {
        if (!Uri.TryCreate(input.BaseUrl.Trim(), UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https") || !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
            throw new ArgumentException("API Base URL phải là URL HTTP(S), không chứa tài khoản, query hoặc fragment.");
        if (!string.IsNullOrWhiteSpace(input.DefaultBranchId) && !Guid.TryParse(input.DefaultBranchId, out _))
            throw new ArgumentException("ID chi nhánh phải là GUID.");
        var url = input.BaseUrl.Trim().TrimEnd('/');
        var old = await GetAsync(ct);
        var changedHost = old != null && !string.Equals(old.BaseUrl, url, StringComparison.OrdinalIgnoreCase);
        if ((old == null || changedHost) && string.IsNullOrWhiteSpace(input.ApiKey))
            throw new ArgumentException("Nhập Partner Key khi tạo kết nối hoặc đổi API Base URL.");
        string? oldKey = null;
        if (old != null)
        {
            try { oldKey = ApiKey(old); }
            catch (System.Security.Cryptography.CryptographicException) { }
        }
        if (old != null && oldKey == null && string.IsNullOrWhiteSpace(input.ApiKey))
            throw new ArgumentException("Không đọc được key đã mã hóa. Nhập lại Partner Key và secret webhook, hoặc khôi phục thư mục .local-data/keys.");
        var keyChanged = !string.IsNullOrWhiteSpace(input.ApiKey) && oldKey != input.ApiKey.Trim();
        var record = new ConnectionRecord
        {
            ConnectionId = old == null || changedHost || keyChanged ? Guid.NewGuid().ToString("N") : old.ConnectionId,
            Name = string.IsNullOrWhiteSpace(input.Name) ? "Cửa hàng thử nghiệm" : input.Name.Trim(),
            BaseUrl = url,
            ProtectedApiKey = !string.IsNullOrWhiteSpace(input.ApiKey) ? _protector.Protect(input.ApiKey.Trim()) : old!.ProtectedApiKey,
            ProtectedWebhookSecret = input.ClearWebhookSecret ? "" : !string.IsNullOrWhiteSpace(input.WebhookSecret) ? _protector.Protect(input.WebhookSecret.Trim()) : changedHost || (old != null && oldKey == null) ? "" : old?.ProtectedWebhookSecret ?? "",
            DefaultBranchId = string.IsNullOrWhiteSpace(input.DefaultBranchId) ? null : input.DefaultBranchId,
            DefaultSaleChannelId = string.IsNullOrWhiteSpace(input.DefaultSaleChannelId) ? "TaiCho" : input.DefaultSaleChannelId.Trim()
        };
        await store.Connections.ReplaceOneAsync(x => x.Id == "current", record, new ReplaceOptions { IsUpsert = true }, ct);
        return record;
    }
}
