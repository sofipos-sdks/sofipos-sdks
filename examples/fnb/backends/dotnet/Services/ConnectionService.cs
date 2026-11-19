using SofiPOS.FnB;
using SofiPOS.FnB.Configuration;
using SofiPOS.FnB.Models;

namespace SofiPOS.FnB.Demo.Services;

public sealed class ConnectionService(DemoStore store, IHttpClientFactory http, ExecutionScope execution)
{
    public Task<ConnectionRecord?> GetAsync(CancellationToken ct = default) => Task.FromResult(store.Connection);

    public Task<ConnectionRecord> RequiredAsync(CancellationToken ct = default)
    {
        var record = store.Connection ?? throw new ConnectionSessionException("Nhập và lưu thông tin kết nối trước khi gọi SDK.");
        if (execution.Current is { Probe: false } scope && scope.ConnectionId != record.ConnectionId)
            throw new ConnectionSessionException("Kết nối đã thay đổi. Tải lại cấu hình trước khi tiếp tục.");
        return Task.FromResult(record);
    }

    public string ApiKey(ConnectionRecord record) => record.ApiKey;
    public string WebhookSecret(ConnectionRecord record) => record.WebhookSecret;

    public static object View(ConnectionRecord? record) => new
    {
        configured = record != null, connectionId = record?.ConnectionId,
        name = record?.Name ?? "Cửa hàng thử nghiệm", baseUrl = record?.BaseUrl ?? "",
        hasApiKey = !string.IsNullOrEmpty(record?.ApiKey), hasWebhookSecret = !string.IsNullOrEmpty(record?.WebhookSecret),
        updatedAt = record?.UpdatedAt
    };

    private string ResolveKey(ConnectionInput input)
    {
        if (!Uri.TryCreate(input.BaseUrl?.Trim(), UriKind.Absolute, out var uri) || uri.Scheme is not ("http" or "https") ||
            !string.IsNullOrEmpty(uri.UserInfo) || !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment))
            throw new ArgumentException("API Base URL phải là URL HTTP(S), không chứa tài khoản, query hoặc fragment.");
        var sameUrl = string.Equals(store.Connection?.BaseUrl, input.BaseUrl.Trim().TrimEnd('/'), StringComparison.OrdinalIgnoreCase);
        var key = !string.IsNullOrWhiteSpace(input.ApiKey) ? input.ApiKey.Trim() : sameUrl ? store.Connection?.ApiKey : null;
        return !string.IsNullOrEmpty(key) ? key : throw new ArgumentException("Nhập Partner Key khi tạo kết nối hoặc đổi API Base URL.");
    }

    public async Task<ConnectionRecord> SaveAsync(ConnectionInput input, CancellationToken ct)
    {
        var key = ResolveKey(input);
        var probe = await ProbeAsync(input, ct);
        var old = store.Connection;
        var url = input.BaseUrl.Trim().TrimEnd('/');
        var changed = old == null || !string.Equals(old.BaseUrl, url, StringComparison.OrdinalIgnoreCase) || old.ApiKey != key;
        var record = new ConnectionRecord
        {
            ConnectionId = changed ? Guid.NewGuid().ToString("N") : old!.ConnectionId,
            Name = string.IsNullOrWhiteSpace(input.Name) ? "Cửa hàng thử nghiệm" : input.Name.Trim(),
            BaseUrl = url, ApiKey = key,
            WebhookSecret = input.ClearWebhookSecret ? "" : !string.IsNullOrWhiteSpace(input.WebhookSecret) ? input.WebhookSecret.Trim() : changed ? "" : old!.WebhookSecret
        };
        if (changed) store.ClearHistory();
        store.Connection = record;
        foreach (var trace in probe.Trace) { trace.ConnectionId = record.ConnectionId; store.AddCall(trace); }
        return record;
    }

    public async Task<ConnectionProbe> ProbeAsync(ConnectionInput input, CancellationToken ct)
    {
        var key = ResolveKey(input);
        var previous = execution.Current;
        var info = new ExecutionInfo { ConnectionId = store.Connection?.ConnectionId ?? "setup", Operation = "Lookups.GetBranchesAsync", Source = "setup", ApiKey = key, Probe = true };
        execution.Current = info;
        try
        {
            using var client = new SofiPosFnBClient(http.CreateClient("sofipos"), new SofiPosOptions { BaseUrl = input.BaseUrl.Trim(), ApiKey = key });
            var branches = await client.Lookups.GetBranchesAsync(ct);
            return new(branches, info.Id, info.ProbeCalls);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception error) { throw new ConnectionProbeException(error, info.Id, info.ProbeCalls); }
        finally { execution.Current = previous; }
    }
}

public sealed record ConnectionProbe(List<Branch> Branches, string CallId, List<ApiCall> Trace);
public sealed class ConnectionSessionException(string message) : InvalidOperationException(message);
public sealed class ConnectionProbeException(Exception cause, string callId, List<ApiCall> trace) : Exception("Kiểm tra kết nối SDK không thành công.", cause)
{
    public string CallId { get; } = callId;
    public List<ApiCall> Trace { get; } = trace;
}
