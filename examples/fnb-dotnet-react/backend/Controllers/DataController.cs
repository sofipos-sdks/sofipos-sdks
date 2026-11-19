using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api")]
public sealed class DataController(DemoStore store, ConnectionService connections) : ControllerBase
{
    [HttpGet("snapshots")]
    public async Task<IActionResult> Snapshots([FromQuery] string? resource, [FromQuery] string? key, [FromQuery] int page = 1, CancellationToken ct = default)
    {
        var c = await connections.RequiredAsync(ct);
        var filter = Builders<Snapshot>.Filter.Eq(x => x.ConnectionId, c.ConnectionId);
        if (!string.IsNullOrWhiteSpace(resource)) filter &= Builders<Snapshot>.Filter.Eq(x => x.Resource, resource);
        if (!string.IsNullOrWhiteSpace(key)) filter &= Builders<Snapshot>.Filter.Regex(x => x.ResourceId, new MongoDB.Bson.BsonRegularExpression(Regex.Escape(key), "i"));
        var count = await store.Snapshots.CountDocumentsAsync(filter, cancellationToken: ct);
        var items = await store.Snapshots.Find(filter).SortByDescending(x => x.UpdatedAt).Skip((Math.Max(1, page) - 1) * 20).Limit(20).ToListAsync(ct);
        var resources = await store.Snapshots.Distinct(x => x.Resource, x => x.ConnectionId == c.ConnectionId).ToListAsync(ct);
        return Ok(new { items, page = Math.Max(1, page), total = count, pageSize = 20, resources });
    }

    [HttpGet("calls")]
    public async Task<IActionResult> Calls([FromQuery] string? operation, [FromQuery] bool errorsOnly = false, [FromQuery] int page = 1, CancellationToken ct = default)
    {
        var c = await connections.RequiredAsync(ct);
        var filter = Builders<ApiCall>.Filter.Eq(x => x.ConnectionId, c.ConnectionId);
        if (!string.IsNullOrWhiteSpace(operation)) filter &= Builders<ApiCall>.Filter.Regex(x => x.Operation, new MongoDB.Bson.BsonRegularExpression(Regex.Escape(operation), "i"));
        if (errorsOnly) filter &= Builders<ApiCall>.Filter.Or(Builders<ApiCall>.Filter.Gte(x => x.StatusCode, 400), Builders<ApiCall>.Filter.Ne(x => x.Error, null));
        var total = await store.Calls.CountDocumentsAsync(filter, cancellationToken: ct);
        var items = await store.Calls.Find(filter).SortByDescending(x => x.CreatedAt).Skip((Math.Max(1, page) - 1) * 20).Limit(20).ToListAsync(ct);
        return Ok(new { items, total, page = Math.Max(1, page), pageSize = 20 });
    }

    [HttpGet("overview")]
    public async Task<IActionResult> Overview(CancellationToken ct)
    {
        var c = await connections.GetAsync(ct);
        if (c == null) return Ok(new { configured = false });
        var id = c.ConnectionId;
        var calls = await store.Calls.Find(x => x.ConnectionId == id).SortByDescending(x => x.CreatedAt).Limit(8).ToListAsync(ct);
        var webhooks = await store.Webhooks.Find(x => x.ConnectionId == id).SortByDescending(x => x.CreatedAt).Limit(6).ToListAsync(ct);
        var snapshots = await store.Snapshots.CountDocumentsAsync(x => x.ConnectionId == id, cancellationToken: ct);
        var callCount = await store.Calls.CountDocumentsAsync(x => x.ConnectionId == id, cancellationToken: ct);
        var webhookCount = await store.Webhooks.CountDocumentsAsync(x => x.ConnectionId == id, cancellationToken: ct);
        var syncs = await store.SyncRuns.Find(x => x.ConnectionId == id).SortByDescending(x => x.CreatedAt).Limit(5).ToListAsync(ct);
        return Ok(new { configured = true, snapshots, callCount, webhookCount, calls, webhooks, syncs });
    }
}
