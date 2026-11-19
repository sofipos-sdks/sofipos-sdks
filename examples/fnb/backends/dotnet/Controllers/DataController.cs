using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api")]
public sealed class DataController(DemoStore store, ConnectionService connections) : ControllerBase
{
    [HttpGet("calls")]
    public async Task<IActionResult> Calls([FromQuery] string? operation, [FromQuery] bool errorsOnly = false,
        [FromQuery] int page = 1, [FromQuery] string? callId = null, CancellationToken ct = default)
    {
        var connection = await connections.RequiredAsync(ct);
        var rows = store.Calls.Where(x => x.ConnectionId == connection.ConnectionId);
        if (!string.IsNullOrEmpty(callId)) rows = rows.Where(x => x.CallId == callId);
        if (!string.IsNullOrWhiteSpace(operation)) rows = rows.Where(x => x.Operation.Contains(operation, StringComparison.OrdinalIgnoreCase));
        if (errorsOnly) rows = rows.Where(x => x.StatusCode >= 400 || x.Error != null);
        var list = rows.ToList();
        page = Math.Clamp(page, 1, DemoStore.HistoryLimit);
        return Ok(new { items = list.Skip((page - 1) * 20).Take(20).ToList(), total = list.Count, page, pageSize = 20, limit = DemoStore.HistoryLimit });
    }
}
