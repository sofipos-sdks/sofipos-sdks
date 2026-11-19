using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class FlashSalesController(SdkGateway gateway) : ControllerBase
{
    [HttpGet("flashsales")]
    public async Task<IActionResult> ListAsync([FromQuery] FlashSaleListQuery? query, CancellationToken ct)
        => Ok(await gateway.CallAsync("FlashSales.ListAsync", (sdk, token) => sdk.FlashSales.ListAsync(query, token), id: null, context: null, ct: ct));

    [HttpGet("flashsales/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.CallAsync("FlashSales.GetAsync", (sdk, token) => sdk.FlashSales.GetAsync(id, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("flashsales")]
    public async Task<IActionResult> CreateAsync([FromBody] FlashSaleRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("FlashSales.CreateAsync", (sdk, token) => sdk.FlashSales.CreateAsync(request, token), id: null, context: null, ct: ct));

    [HttpPut("flashsales/{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] FlashSaleRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("FlashSales.UpdateAsync", (sdk, token) => sdk.FlashSales.UpdateAsync(id, request, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("flashsales/{id:guid}/delete")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.VoidAsync("FlashSales.DeleteAsync", (sdk, token) => sdk.FlashSales.DeleteAsync(id, token), id: id.ToString(), ct: ct));
}
