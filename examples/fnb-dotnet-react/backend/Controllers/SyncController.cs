using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using SofiPOS.FnB.Demo.Services;
using SofiPOS.FnB.Http;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/sync")]
public sealed class SyncController(DemoStore store, ConnectionService connections) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(CancellationToken ct)
    {
        var c = await connections.RequiredAsync(ct);
        return Ok(await store.SyncRuns.Find(x => x.ConnectionId == c.ConnectionId).SortByDescending(x => x.CreatedAt).Limit(40).ToListAsync(ct));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id, CancellationToken ct)
    {
        var c = await connections.RequiredAsync(ct);
        var run = await store.SyncRuns.Find(x => x.Id == id && x.ConnectionId == c.ConnectionId).FirstOrDefaultAsync(ct);
        return run == null ? NotFound() : Ok(run);
    }

    [HttpPost]
    public async Task<IActionResult> Start([FromBody] SyncInput input, CancellationToken ct)
    {
        var c = await connections.RequiredAsync(ct);
        input.Resources = input.Resources.Distinct().ToList();
        if (input.Resources.Count == 0 || input.Resources.Except(SyncService.Resources).Any()) throw new ArgumentException("Chọn ít nhất một nhóm đồng bộ hợp lệ.");
        input.BranchId ??= c.DefaultBranchId;
        if (!string.IsNullOrEmpty(input.BranchId) && !Guid.TryParse(input.BranchId, out _)) throw new ArgumentException("Chi nhánh phải là GUID.");
        if (input.Resources.Intersect(new[] { "orders", "menu", "areas", "payment-options" }).Any() && string.IsNullOrEmpty(input.BranchId)) throw new ArgumentException("Chọn chi nhánh FnB trước khi đồng bộ dữ liệu bán hàng.");
        if (input.Resources.Contains("orders"))
        {
            if (string.IsNullOrEmpty(input.FromDate) || string.IsNullOrEmpty(input.ToDate)) throw new ArgumentException("Đồng bộ đơn cần khoảng thời gian từ ngày / đến ngày.");
            if (VietnamDateTimeConverter.Parse(input.FromDate) > VietnamDateTimeConverter.Parse(input.ToDate)) throw new ArgumentException("Ngày kết thúc phải sau ngày bắt đầu.");
        }
        var run = new SyncRun { ConnectionId = c.ConnectionId, Input = input };
        await store.SyncRuns.InsertOneAsync(run, cancellationToken: ct);
        return Accepted($"/api/sync/{run.Id}", run);
    }
}
