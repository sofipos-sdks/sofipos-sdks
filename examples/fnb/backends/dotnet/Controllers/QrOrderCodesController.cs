using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class QrOrderCodesController(SdkGateway gateway) : ControllerBase
{
    [HttpGet("qr-order-codes/branches")]
    public async Task<IActionResult> GetBranchesAsync(CancellationToken ct)
        => Ok(await gateway.CallAsync("QrOrderCodes.GetBranchesAsync", (sdk, token) => sdk.QrOrderCodes.GetBranchesAsync(token), id: null, context: null, ct: ct));

    [HttpGet("qr-order-codes/sale-channels")]
    public async Task<IActionResult> GetSaleChannelsAsync(CancellationToken ct)
        => Ok(await gateway.CallAsync("QrOrderCodes.GetSaleChannelsAsync", (sdk, token) => sdk.QrOrderCodes.GetSaleChannelsAsync(token), id: null, context: null, ct: ct));

    [HttpGet("qr-order-codes/tables")]
    public async Task<IActionResult> GetTablesAsync([FromQuery] Guid branchId, CancellationToken ct)
        => Ok(await gateway.CallAsync("QrOrderCodes.GetTablesAsync", (sdk, token) => sdk.QrOrderCodes.GetTablesAsync(branchId, token), id: null, context: null, ct: ct));

    [HttpGet("qr-order-codes/order-attributes")]
    public async Task<IActionResult> GetOrderAttributesAsync(CancellationToken ct)
        => Ok(await gateway.CallAsync("QrOrderCodes.GetOrderAttributesAsync", (sdk, token) => sdk.QrOrderCodes.GetOrderAttributesAsync(token), id: null, context: null, ct: ct));

    [HttpGet("qr-order-codes")]
    public async Task<IActionResult> ListAsync([FromQuery] QrOrderCodeListQuery? query, CancellationToken ct)
        => Ok(await gateway.CallAsync("QrOrderCodes.ListAsync", (sdk, token) => sdk.QrOrderCodes.ListAsync(query, token), id: null, context: null, ct: ct));

    [HttpGet("qr-order-codes/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.CallAsync("QrOrderCodes.GetAsync", (sdk, token) => sdk.QrOrderCodes.GetAsync(id, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("qr-order-codes")]
    public async Task<IActionResult> CreateAsync([FromBody] QrOrderCodeRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("QrOrderCodes.CreateAsync", (sdk, token) => sdk.QrOrderCodes.CreateAsync(request, token), id: null, context: null, ct: ct));

    [HttpPut("qr-order-codes/{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] QrOrderCodeRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("QrOrderCodes.UpdateAsync", (sdk, token) => sdk.QrOrderCodes.UpdateAsync(id, request, token), id: id.ToString(), context: null, ct: ct));

    [HttpPut("qr-order-codes/{id:guid}/active")]
    public async Task<IActionResult> SetActiveAsync([FromRoute] Guid id, [FromBody] QrOrderCodesSetActiveBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("QrOrderCodes.SetActiveAsync", (sdk, token) => sdk.QrOrderCodes.SetActiveAsync(id, body.IsActive, token), id: id.ToString(), context: null, ct: ct));

    [HttpGet("qr-order-codes/{id:guid}/qr")]
    public async Task<IActionResult> GetQrImageAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.CallAsync("QrOrderCodes.GetQrImageAsync", (sdk, token) => sdk.QrOrderCodes.GetQrImageAsync(id, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("qr-order-codes/{id:guid}/delete")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.VoidAsync("QrOrderCodes.DeleteAsync", (sdk, token) => sdk.QrOrderCodes.DeleteAsync(id, token), id: id.ToString(), ct: ct));
}
