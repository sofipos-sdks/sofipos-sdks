using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using SofiPOS.FnB;
using SofiPOS.FnB.Configuration;
using SofiPOS.FnB.Http;
using SofiPOS.FnB.Exceptions;

namespace SofiPOS.FnB.Demo.Services;

public sealed record SdkResult<T>(T Data, string CallId, string? SyncWarning);

public sealed class SdkGateway(ConnectionService connections, IHttpClientFactory http, DemoStore store, ExecutionScope execution)
{
    public async Task<SdkResult<T>> CallAsync<T>(string operation, Func<SofiPosFnBClient, CancellationToken, Task<T>> action,
        string? id = null, string? context = null, string source = "manual", CancellationToken ct = default)
    {
        var connection = await connections.RequiredAsync(ct);
        var key = connections.ApiKey(connection);
        var info = new ExecutionInfo { ConnectionId = connection.ConnectionId, Operation = operation, Source = source, ApiKey = key };
        var previous = execution.Current;
        execution.Current = info;
        try
        {
            using var client = new SofiPosFnBClient(http.CreateClient("sofipos"), new SofiPosOptions { BaseUrl = connection.BaseUrl, ApiKey = key });
            var data = await action(client, ct);
            string? warning = null;
            try
            {
                await MirrorAsync(connection, operation, data, id, context, CancellationToken.None);
                if (operation == "FnbOrders.MergeAsync" && id != null)
                    await store.DeleteSnapshotAsync(connection.ConnectionId, "orders", id, CancellationToken.None);
                if (operation == "FnbOrders.MoveItemsAsync")
                {
                    var moved = JsonSerializer.SerializeToElement(data, SofiPosJson.Options);
                    foreach (var name in new[] { "sourceOrderId", "targetOrderId" })
                        if (moved.TryGetProperty(name, out var v) && Guid.TryParse(v.GetString(), out var orderId))
                        {
                            try { await CallAsync("FnbOrders.GetAsync", (sdk, token) => sdk.FnbOrders.GetAsync(orderId, token), id: orderId.ToString(), source: "refresh", ct: CancellationToken.None); }
                            catch (SofiPosApiException ex) when ((int)ex.StatusCode == 404 || (int)ex.StatusCode == 422)
                            {
                                // Đơn nguồn rỗng đã bị xóa sau khi chuyển hết món.
                                await store.DeleteSnapshotAsync(connection.ConnectionId, "orders", orderId.ToString(), CancellationToken.None);
                            }
                        }
                }
            }
            catch { warning = "POS đã xử lý thành công nhưng chưa cập nhật được bản sao MongoDB. Hãy đồng bộ lại, không gửi lại thao tác ghi."; }
            if (warning != null && source is "sync" or "webhook") throw new InvalidOperationException(warning);
            return new(data, info.Id, warning ?? info.LogWarning);
        }
        finally { execution.Current = previous; }
    }

    public Task<SdkResult<object?>> VoidAsync(string operation, Func<SofiPosFnBClient, CancellationToken, Task> action,
        string? id = null, CancellationToken ct = default)
        => CallAsync<object?>(operation, async (client, token) => { await action(client, token); return null; }, id, ct: ct);

    public async Task EnumerateAsync<T>(string operation, Func<SofiPosFnBClient, CancellationToken, IAsyncEnumerable<T>> source,
        Func<T, CancellationToken, Task> consume, CancellationToken ct)
    {
        var connection = await connections.RequiredAsync(ct);
        var key = connections.ApiKey(connection);
        var previous = execution.Current;
        execution.Current = new ExecutionInfo { ConnectionId = connection.ConnectionId, Operation = operation, Source = "sync", ApiKey = key };
        try
        {
            using var client = new SofiPosFnBClient(http.CreateClient("sofipos"), new SofiPosOptions { BaseUrl = connection.BaseUrl, ApiKey = key });
            await foreach (var item in source(client, ct).WithCancellation(ct)) await consume(item, ct);
        }
        finally { execution.Current = previous; }
    }

    private async Task MirrorAsync<T>(ConnectionRecord connection, string operation, T data, string? hint, string? context, CancellationToken ct)
    {
        var (service, method) = (operation.Split('.')[0], operation.Split('.')[1]);
        var resource = ResourceFor(service);
        if (method == "DeleteAsync")
        {
            if (hint != null) await store.DeleteSnapshotAsync(connection.ConnectionId, resource, hint, ct);
            return;
        }
        if (data == null) return;
        if (operation == "FnbOrders.MoveItemsAsync") return;
        var value = JsonSerializer.SerializeToElement(data, SofiPosJson.Options);
        var rootMethod = method is "ListAsync" or "GetAsync" or "CreateAsync" or "UpdateAsync" or "SetActiveAsync";
        if (service == "FnbOrders" || (service == "FnbPayments" && method == "PayAsync"))
        {
            resource = "orders";
            rootMethod = true;
        }
        if (!rootMethod || service is "Lookups" or "Fnb")
        {
            resource = service switch
            {
                "Fnb" => method switch { "GetMenuAsync" => "menu", "GetAreasAsync" => "areas", "GetPaymentOptionsAsync" => "payment-options", _ => $"fnb.{method.Replace("Async", "")}" },
                "Lookups" => method == "GetBranchesAsync" ? "branches" : "membership-ranks",
                _ => $"{resource}.{method.Replace("Async", "")}"
            };
            await store.SaveSnapshotAsync(connection.ConnectionId, resource, hint ?? "all", context ?? connection.DefaultBranchId ?? "default", value.GetRawText(), false, ct);
            return;
        }
        if (value.ValueKind == JsonValueKind.Object && value.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in items.EnumerateArray())
                await store.SaveSnapshotAsync(connection.ConnectionId, resource, RecordId(item, null), "", item.GetRawText(), true, ct);
        }
        else if (value.ValueKind == JsonValueKind.Object)
            await store.SaveSnapshotAsync(connection.ConnectionId, resource, RecordId(value, hint), "", value.GetRawText(), false, ct);
    }

    public static string ResourceFor(string service) => service switch
    {
        "Products" => "products", "QrOrderCodes" => "qr-order-codes", "Customers" => "customers",
        "PricingPolicies" => "pricing-policies", "SaleChannels" => "sale-channels", "Combos" => "combos",
        "FlashSales" => "flashsales", "Vouchers" => "vouchers", "OrderAdjustmentPrograms" => "order-adjustment-programs",
        "FnbOrders" => "orders", _ => service.ToLowerInvariant()
    };

    private static string RecordId(JsonElement value, string? fallback)
    {
        foreach (var name in new[] { "id", "productId", "orderId", "saleChannelId" })
            if (value.TryGetProperty(name, out var id) && id.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(id.GetString())) return id.GetString()!;
        return fallback ?? Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value.GetRawText())))[..24];
    }
}
