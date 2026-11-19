using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class SaleChannelsController(SdkGateway gateway) : ControllerBase
{
    [HttpPut("sale-channels/{saleChannelId}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] string saleChannelId, [FromBody] SaleChannelUpdateRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("SaleChannels.UpdateAsync", (sdk, token) => sdk.SaleChannels.UpdateAsync(saleChannelId, request, token), id: saleChannelId, ct: ct));

    [HttpGet("sale-channels/options")]
    public async Task<IActionResult> GetOptionsAsync(CancellationToken ct)
        => Ok(await gateway.CallAsync("SaleChannels.GetOptionsAsync", (sdk, token) => sdk.SaleChannels.GetOptionsAsync(token), id: null, context: null, ct: ct));

    [HttpGet("sale-channels")]
    public async Task<IActionResult> ListAsync([FromQuery] SaleChannelListQuery? query, CancellationToken ct)
        => Ok(await gateway.CallAsync("SaleChannels.ListAsync", (sdk, token) => sdk.SaleChannels.ListAsync(query, token), id: null, context: null, ct: ct));

    [HttpGet("sale-channels/{saleChannelId}")]
    public async Task<IActionResult> GetAsync([FromRoute] string saleChannelId, CancellationToken ct)
        => Ok(await gateway.CallAsync("SaleChannels.GetAsync", (sdk, token) => sdk.SaleChannels.GetAsync(saleChannelId, token), id: saleChannelId.ToString(), context: null, ct: ct));

    [HttpPost("sale-channels")]
    public async Task<IActionResult> CreateAsync([FromBody] SaleChannelCreateRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("SaleChannels.CreateAsync", (sdk, token) => sdk.SaleChannels.CreateAsync(request, token), id: null, context: null, ct: ct));

    [HttpPost("sale-channels/{saleChannelId}/delete")]
    public async Task<IActionResult> DeleteAsync([FromRoute] string saleChannelId, CancellationToken ct)
        => Ok(await gateway.VoidAsync("SaleChannels.DeleteAsync", (sdk, token) => sdk.SaleChannels.DeleteAsync(saleChannelId, token), id: saleChannelId.ToString(), ct: ct));
}
