using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class VouchersController(SdkGateway gateway) : ControllerBase
{
    [HttpGet("vouchers")]
    public async Task<IActionResult> ListAsync([FromQuery] VoucherListQuery? query, CancellationToken ct)
        => Ok(await gateway.CallAsync("Vouchers.ListAsync", (sdk, token) => sdk.Vouchers.ListAsync(query, token), id: null, context: null, ct: ct));

    [HttpGet("vouchers/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.CallAsync("Vouchers.GetAsync", (sdk, token) => sdk.Vouchers.GetAsync(id, token), id: id.ToString(), context: null, ct: ct));

    [HttpGet("vouchers/{id:guid}/codes")]
    public async Task<IActionResult> GetCodesAsync([FromRoute] Guid id, [FromQuery] VoucherCodeListQuery? query, CancellationToken ct)
        => Ok(await gateway.CallAsync("Vouchers.GetCodesAsync", (sdk, token) => sdk.Vouchers.GetCodesAsync(id, query, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("vouchers")]
    public async Task<IActionResult> CreateAsync([FromBody] VoucherRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("Vouchers.CreateAsync", (sdk, token) => sdk.Vouchers.CreateAsync(request, token), id: null, context: null, ct: ct));

    [HttpPut("vouchers/{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] VoucherRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("Vouchers.UpdateAsync", (sdk, token) => sdk.Vouchers.UpdateAsync(id, request, token), id: id.ToString(), context: null, ct: ct));

    [HttpPut("vouchers/{id:guid}/active")]
    public async Task<IActionResult> SetActiveAsync([FromRoute] Guid id, [FromBody] VouchersSetActiveBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("Vouchers.SetActiveAsync", (sdk, token) => sdk.Vouchers.SetActiveAsync(id, body.IsActive, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("vouchers/{id:guid}/delete")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.VoidAsync("Vouchers.DeleteAsync", (sdk, token) => sdk.Vouchers.DeleteAsync(id, token), id: id.ToString(), ct: ct));
}
