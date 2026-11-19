using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class FnbController(SdkGateway gateway) : ControllerBase
{
    [HttpGet("fnb/areas")]
    public async Task<IActionResult> GetAreasAsync([FromQuery] Guid branchId, CancellationToken ct)
        => Ok(await gateway.CallAsync("Fnb.GetAreasAsync", (sdk, token) => sdk.Fnb.GetAreasAsync(branchId, token), id: null, context: string.Join("/", branchId.ToString(), ""), ct: ct));

    [HttpGet("fnb/menu")]
    public async Task<IActionResult> GetMenuAsync([FromQuery] Guid branchId, [FromQuery] string? saleChannelId, CancellationToken ct)
        => Ok(await gateway.CallAsync("Fnb.GetMenuAsync", (sdk, token) => sdk.Fnb.GetMenuAsync(branchId, saleChannelId, token), id: null, context: string.Join("/", branchId.ToString(), saleChannelId ?? "TaiCho"), ct: ct));

    [HttpGet("fnb/products/{productId:guid}/addons")]
    public async Task<IActionResult> GetProductAddOnsAsync([FromRoute] Guid productId, [FromQuery] Guid branchId, CancellationToken ct)
        => Ok(await gateway.CallAsync("Fnb.GetProductAddOnsAsync", (sdk, token) => sdk.Fnb.GetProductAddOnsAsync(branchId, productId, token), id: productId.ToString(), context: string.Join("/", branchId.ToString(), ""), ct: ct));

    [HttpGet("fnb/products/{productId:guid}/attributes")]
    public async Task<IActionResult> GetProductAttributesAsync([FromRoute] Guid productId, [FromQuery] Guid branchId, CancellationToken ct)
        => Ok(await gateway.CallAsync("Fnb.GetProductAttributesAsync", (sdk, token) => sdk.Fnb.GetProductAttributesAsync(branchId, productId, token), id: productId.ToString(), context: string.Join("/", branchId.ToString(), ""), ct: ct));

    [HttpGet("fnb/payment-options")]
    public async Task<IActionResult> GetPaymentOptionsAsync([FromQuery] Guid branchId, CancellationToken ct)
        => Ok(await gateway.CallAsync("Fnb.GetPaymentOptionsAsync", (sdk, token) => sdk.Fnb.GetPaymentOptionsAsync(branchId, token), id: null, context: string.Join("/", branchId.ToString(), ""), ct: ct));
}
