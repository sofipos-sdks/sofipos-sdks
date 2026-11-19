using System.Diagnostics;
using System.Text.Json.Nodes;

namespace SofiPOS.FnB.Demo.Services;

public sealed class ExecutionInfo
{
    public string Id { get; } = Guid.NewGuid().ToString("N");
    public string ConnectionId { get; init; } = "";
    public string Operation { get; init; } = "";
    public string Source { get; init; } = "manual";
    public string ApiKey { get; init; } = "";
    public string? LogWarning { get; set; }
}

public sealed class ExecutionScope
{
    private readonly AsyncLocal<ExecutionInfo?> _value = new();
    public ExecutionInfo? Current { get => _value.Value; set => _value.Value = value; }
}

public sealed class SdkTraceHandler(DemoStore store, ExecutionScope execution) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var info = execution.Current;
        var record = new ApiCall
        {
            ConnectionId = info?.ConnectionId ?? "",
            CallId = info?.Id ?? "",
            Operation = info?.Operation ?? "",
            Source = info?.Source ?? "manual",
            Method = request.Method.Method,
            Url = request.RequestUri?.ToString() ?? "",
            Headers = request.Headers.ToDictionary(x => x.Key, x => x.Key.Contains("key", StringComparison.OrdinalIgnoreCase) || x.Key.Contains("authorization", StringComparison.OrdinalIgnoreCase) ? "[đã ẩn]" : string.Join(", ", x.Value))
        };
        if (request.Content != null) record.RequestJson = SafeJson(await request.Content.ReadAsStringAsync(cancellationToken), info?.ApiKey);
        var timer = Stopwatch.StartNew();
        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            record.StatusCode = (int)response.StatusCode;
            record.TraceId = response.Headers.TryGetValues("trace-id", out var ids) ? ids.FirstOrDefault() : null;
            record.ResponseJson = SafeJson(await response.Content.ReadAsStringAsync(cancellationToken), info?.ApiKey);
            return response;
        }
        catch (Exception error)
        {
            record.Error = error is OperationCanceledException ? "Quá thời gian chờ hoặc request đã hủy." : "Lỗi kết nối API POS.";
            throw;
        }
        finally
        {
            record.DurationMs = timer.ElapsedMilliseconds;
            try { await store.Calls.InsertOneAsync(record, cancellationToken: CancellationToken.None); }
            catch { if (info != null) info.LogWarning = "API đã phản hồi nhưng chưa lưu được lịch sử vào MongoDB."; }
        }
    }

    public static string SafeJson(string value, string? apiKey = null)
    {
        if (!string.IsNullOrEmpty(apiKey)) value = value.Replace(apiKey, "[đã ẩn]", StringComparison.Ordinal);
        try
        {
            var node = JsonNode.Parse(value);
            Redact(node);
            value = node?.ToJsonString() ?? value;
        }
        catch (System.Text.Json.JsonException) { }
        return value.Length > 200_000 ? value[..200_000] + "\n[Đã rút gọn log lớn hơn 200 KB]" : value;
    }

    private static void Redact(JsonNode? node)
    {
        if (node is JsonObject obj)
            foreach (var key in obj.Select(x => x.Key).ToList())
            {
                if (new[] { "apiKey", "partnerKey", "webhookSecret", "secret", "authorization", "protectedApiKey", "protectedWebhookSecret" }.Contains(key, StringComparer.OrdinalIgnoreCase)) obj[key] = "[đã ẩn]";
                else if (key.Equals("base64data", StringComparison.OrdinalIgnoreCase)) obj[key] = "[Nội dung ảnh đã ẩn khỏi log]";
                else Redact(obj[key]);
            }
        else if (node is JsonArray array) foreach (var item in array) Redact(item);
    }
}
