using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class OrderAdjustmentProgramsController(SdkGateway gateway) : ControllerBase
{
    [HttpGet("order-adjustment-programs")]
    public async Task<IActionResult> ListAsync([FromQuery] OrderAdjustmentProgramListQuery? query, CancellationToken ct)
        => Ok(await gateway.CallAsync("OrderAdjustmentPrograms.ListAsync", (sdk, token) => sdk.OrderAdjustmentPrograms.ListAsync(query, token), id: null, context: null, ct: ct));

    [HttpGet("order-adjustment-programs/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.CallAsync("OrderAdjustmentPrograms.GetAsync", (sdk, token) => sdk.OrderAdjustmentPrograms.GetAsync(id, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("order-adjustment-programs")]
    public async Task<IActionResult> CreateAsync([FromBody] OrderAdjustmentProgramRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("OrderAdjustmentPrograms.CreateAsync", (sdk, token) => sdk.OrderAdjustmentPrograms.CreateAsync(request, token), id: null, context: null, ct: ct));

    [HttpPut("order-adjustment-programs/{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] OrderAdjustmentProgramRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("OrderAdjustmentPrograms.UpdateAsync", (sdk, token) => sdk.OrderAdjustmentPrograms.UpdateAsync(id, request, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("order-adjustment-programs/{id:guid}/delete")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.VoidAsync("OrderAdjustmentPrograms.DeleteAsync", (sdk, token) => sdk.OrderAdjustmentPrograms.DeleteAsync(id, token), id: id.ToString(), ct: ct));
}
