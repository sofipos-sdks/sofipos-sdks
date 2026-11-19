using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Demo.Services;
using SofiPOS.FnB.Http;
using SofiPOS.FnB.Webhooks;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
public sealed class WebhooksController(DemoStore store, ConnectionService connections) : ControllerBase
{
    [HttpPost("/webhooks/sofipos")]
    [RequestSizeLimit(1_048_576)]
    public async Task<IActionResult> Receive(CancellationToken ct)
    {
        using var buffer = new MemoryStream();
        await Request.Body.CopyToAsync(buffer, ct);
        return await AcceptAsync(buffer.ToArray(), Request.Headers[SofiPosWebhookHeaders.Timestamp].ToString(),
            Request.Headers[SofiPosWebhookHeaders.Signature].ToString(), Request.Headers[SofiPosWebhookHeaders.Id].ToString(),
            Request.Headers[SofiPosWebhookHeaders.Event].ToString(), ct);
    }

    [HttpGet("/api/webhooks")]
    public async Task<IActionResult> List([FromQuery] string? state, [FromQuery] int page = 1, CancellationToken ct = default)
    {
        var connection = await connections.RequiredAsync(ct);
        var rows = store.Webhooks.Where(x => x.ConnectionId == connection.ConnectionId);
        if (!string.IsNullOrWhiteSpace(state)) rows = rows.Where(x => x.State == state);
        var list = rows.ToList();
        page = Math.Clamp(page, 1, DemoStore.HistoryLimit);
        return Ok(new { items = list.Skip((page - 1) * 20).Take(20).ToList(), total = list.Count, page, pageSize = 20, limit = DemoStore.HistoryLimit });
    }

    [HttpPost("/api/webhooks/simulate")]
    public async Task<IActionResult> Simulate([FromBody] WebhookSimulation input, CancellationToken ct)
    {
        var connection = await connections.RequiredAsync(ct);
        var secret = connection.WebhookSecret;
        if (string.IsNullOrEmpty(secret)) throw new InvalidOperationException("Nhập secret webhook trong trang Kết nối trước.");
        if (input.SignatureMode is not ("valid" or "invalid" or "expired")) throw new ArgumentException("Chọn chế độ chữ ký hợp lệ.");
        var id = string.IsNullOrEmpty(input.EventId) ? Guid.NewGuid().ToString("N") : input.EventId;
        object data = input.Event == "ping" ? new { partnerId = Guid.Empty, message = "Webhook mô phỏng cục bộ từ SDK .NET" } :
            input.Event.StartsWith("order.", StringComparison.Ordinal) ? new { orderId = input.OrderId, code = input.Code, branchId = input.BranchId } :
            new { ids = input.Ids, deletedIds = input.DeletedIds };
        var raw = JsonSerializer.SerializeToUtf8Bytes(new { id, @event = input.Event, createdAt = DateTimeOffset.UtcNow, data }, SofiPosJson.Options);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - (input.SignatureMode == "expired" ? 600 : 0);
        var signature = SofiPosWebhookVerifier.ComputeSignature(secret, timestamp, raw);
        if (input.SignatureMode == "invalid") signature = "sha256=" + new string('0', 64);
        Response.Headers["X-Demo-Event-Id"] = id;
        return await AcceptAsync(raw, timestamp.ToString(System.Globalization.CultureInfo.InvariantCulture), signature, id, input.Event, ct, true);
    }

    private async Task<IActionResult> AcceptAsync(byte[] raw, string timestamp, string signature, string headerId,
        string headerEvent, CancellationToken ct, bool simulated = false)
    {
        var connection = await connections.RequiredAsync(ct);
        if (string.IsNullOrEmpty(connection.WebhookSecret)) return Conflict(new { message = "Chưa cấu hình secret webhook." });
        var headers = Request.Headers.ToDictionary(h => h.Key,
            h => new[] { "authorization", "key", "token", "secret", "cookie" }.Any(word => h.Key.Contains(word, StringComparison.OrdinalIgnoreCase)) ? "[đã ẩn]" : h.Value.ToString());
        headers[SofiPosWebhookHeaders.Id] = headerId;
        headers[SofiPosWebhookHeaders.Event] = headerEvent;
        headers[SofiPosWebhookHeaders.Timestamp] = timestamp;
        headers[SofiPosWebhookHeaders.Signature] = signature;
        SofiPosWebhookEvent parsed;
        try
        {
            parsed = SofiPosWebhookVerifier.VerifyAndParse(connection.WebhookSecret, timestamp, signature, raw);
            if (parsed.Id != headerId || parsed.Event != headerEvent) throw new ArgumentException("ID hoặc loại sự kiện trong header không khớp body đã ký.");
        }
        catch (Exception error) when (error is SofiPosWebhookException or ArgumentException or JsonException)
        {
            store.AddWebhook(new WebhookRecord { ConnectionId = connection.ConnectionId, EventId = headerId, Event = headerEvent,
                PayloadJson = SdkTraceHandler.SafeJson(Encoding.UTF8.GetString(raw), connection.ApiKey), SignatureStatus = "invalid",
                State = "rejected", Error = error.Message, Headers = headers, Source = simulated ? "simulated" : "pos" });
            return Unauthorized(new { message = error.Message });
        }
        var payload = Encoding.UTF8.GetString(raw);
        if (payload.Length > 200_000) payload = payload[..200_000] + "\n[Đã rút gọn payload lớn hơn 200 KB]";
        var inserted = store.AddWebhook(new WebhookRecord { ConnectionId = connection.ConnectionId, EventId = parsed.Id,
            Event = parsed.Event, PayloadJson = payload, Headers = headers, Source = simulated ? "simulated" : "pos" });
        var result = new { eventId = parsed.Id, duplicate = !inserted,
            message = inserted ? "Đã xác minh và nhận webhook trong phiên. Không gọi API POS hoặc đồng bộ dữ liệu." : "Đã nhận sự kiện trước đó; tăng số lần nhận." };
        return inserted ? Accepted(result) : Ok(result);
    }
}
