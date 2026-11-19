using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
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
            Request.Headers[SofiPosWebhookHeaders.Signature].ToString(), Request.Headers[SofiPosWebhookHeaders.Id].ToString(), Request.Headers[SofiPosWebhookHeaders.Event].ToString(), ct);
    }

    [HttpGet("/api/webhooks")]
    public async Task<IActionResult> List([FromQuery] string? state, [FromQuery] int page = 1, CancellationToken ct = default)
    {
        var c=await connections.RequiredAsync(ct);
        var filter=Builders<WebhookRecord>.Filter.Eq(x=>x.ConnectionId,c.ConnectionId);
        if (!string.IsNullOrWhiteSpace(state))filter &= Builders<WebhookRecord>.Filter.Eq(x=>x.State,state);
        var total=await store.Webhooks.CountDocumentsAsync(filter,cancellationToken:ct);
        var items=await store.Webhooks.Find(filter).SortByDescending(x=>x.CreatedAt).Skip((Math.Max(1,page)-1)*20).Limit(20).ToListAsync(ct);
        return Ok(new {items,total,page=Math.Max(1,page),pageSize=20});
    }

    [HttpPost("/api/webhooks/{id}/reprocess")]
    public async Task<IActionResult> Reprocess(string id,CancellationToken ct)
    {
        var c=await connections.RequiredAsync(ct);
        var update=Builders<WebhookRecord>.Update.Set(x=>x.State,"pending").Set(x=>x.Error,null).Set(x=>x.LeaseUntil,null).Set(x=>x.Attempts,0);
        var result=await store.Webhooks.UpdateOneAsync(x=>x.Id==id && x.ConnectionId==c.ConnectionId && x.SignatureStatus=="valid" && x.State!="running",update,cancellationToken:ct);
        return result.MatchedCount==0 ? NotFound(new {message="Không có sự kiện hợp lệ để xử lý lại, hoặc sự kiện đang chạy."}) : Accepted(new {message="Đã đưa sự kiện vào hàng chờ."});
    }

    [HttpPost("/api/webhooks/simulate")]
    public async Task<IActionResult> Simulate([FromBody] WebhookSimulation input,CancellationToken ct)
    {
        var c=await connections.RequiredAsync(ct);
        var secret=connections.WebhookSecret(c);
        if (string.IsNullOrEmpty(secret))throw new InvalidOperationException("Nhập secret webhook trong trang Kết nối trước.");
        if(input.SignatureMode is not ("valid" or "invalid" or "expired"))throw new ArgumentException("Chọn chế độ chữ ký hợp lệ.");
        var id=string.IsNullOrEmpty(input.EventId)?Guid.NewGuid().ToString("N"):input.EventId;
        object data=input.Event=="ping"?new{partnerId=Guid.Empty,message="Webhook mô phỏng cục bộ từ SDK .NET"}:
            input.Event.StartsWith("order.",StringComparison.Ordinal)?new{orderId=input.OrderId,code=input.Code,branchId=input.BranchId}:
            new{ids=input.Ids,deletedIds=input.DeletedIds};
        var raw=JsonSerializer.SerializeToUtf8Bytes(new{id,@event=input.Event,createdAt=DateTimeOffset.UtcNow,data},SofiPosJson.Options);
        var timestamp=DateTimeOffset.UtcNow.ToUnixTimeSeconds()-(input.SignatureMode=="expired"?600:0);
        var signature=SofiPosWebhookVerifier.ComputeSignature(secret,timestamp,raw);
        if(input.SignatureMode=="invalid")signature="sha256="+new string('0',64);
        Response.Headers["X-Demo-Event-Id"]=id;
        return await AcceptAsync(raw,timestamp.ToString(System.Globalization.CultureInfo.InvariantCulture),signature,id,input.Event,ct,true);
    }

    private async Task<IActionResult> AcceptAsync(byte[] raw,string timestamp,string signature,string headerId,string headerEvent,CancellationToken ct,bool simulated=false)
    {
        var c=await connections.RequiredAsync(ct);
        var secret=connections.WebhookSecret(c);
        if(string.IsNullOrEmpty(secret))return StatusCode(409,new{message="Chưa cấu hình secret webhook."});
        var headers = Request.Headers.ToDictionary(h => h.Key, h => new[] { "authorization", "key", "token", "secret", "cookie" }.Any(word => h.Key.Contains(word,StringComparison.OrdinalIgnoreCase)) ? "[đã ẩn]" : h.Value.ToString());
        headers[SofiPosWebhookHeaders.Id]=headerId;headers[SofiPosWebhookHeaders.Event]=headerEvent;
        headers[SofiPosWebhookHeaders.Timestamp]=timestamp;headers[SofiPosWebhookHeaders.Signature]=signature;
        SofiPosWebhookEvent parsed;
        try
        {
            parsed=SofiPosWebhookVerifier.VerifyAndParse(secret,timestamp,signature,raw);
            if(parsed.Id!=headerId || parsed.Event!=headerEvent)throw new ArgumentException("ID hoặc loại sự kiện trong header không khớp body đã ký.");
        }
        catch(Exception ex) when(ex is SofiPosWebhookException or ArgumentException or JsonException)
        {
            var rejection=new WebhookRecord
            {
                Id=Guid.NewGuid().ToString("N"),ConnectionId=c.ConnectionId,EventId="invalid-"+Guid.NewGuid().ToString("N"),
                Event=headerEvent,PayloadJson=SdkTraceHandler.SafeJson(Encoding.UTF8.GetString(raw)),SignatureStatus="invalid",State="rejected",Error=ex.Message,Headers=headers,Source=simulated?"simulated":"pos"
            };
            await store.Webhooks.InsertOneAsync(rejection,cancellationToken:ct);
            return Unauthorized(new{message=ex.Message});
        }
        var record=new WebhookRecord{Id=c.ConnectionId+":"+parsed.Id,ConnectionId=c.ConnectionId,EventId=parsed.Id,Event=parsed.Event,PayloadJson=Encoding.UTF8.GetString(raw),Headers=headers,Source=simulated?"simulated":"pos"};
        try { await store.Webhooks.InsertOneAsync(record,cancellationToken:ct); }
        catch(MongoWriteException ex) when(ex.WriteError.Category==ServerErrorCategory.DuplicateKey)
        {
            await store.Webhooks.UpdateOneAsync(x=>x.Id==record.Id,Builders<WebhookRecord>.Update.Inc(x=>x.ReceivedCount,1),cancellationToken:ct);
            return Ok(new{eventId=record.EventId,duplicate=true,message="Đã nhận sự kiện trước đó; không xử lý trùng."});
        }
        return Accepted(new{eventId=record.EventId,duplicate=false,message="Đã lưu webhook, đang chờ đồng bộ."});
    }
}
