using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class ProductsController(SdkGateway gateway) : ControllerBase
{
    [HttpGet("products")]
    public async Task<IActionResult> ListAsync([FromQuery] ProductListQuery? query, CancellationToken ct)
        => Ok(await gateway.CallAsync("Products.ListAsync", (sdk, token) => sdk.Products.ListAsync(query, token), id: null, context: null, ct: ct));

    [HttpGet("products/{productId:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid productId, CancellationToken ct)
        => Ok(await gateway.CallAsync("Products.GetAsync", (sdk, token) => sdk.Products.GetAsync(productId, token), id: productId.ToString(), context: null, ct: ct));

    [HttpGet("products/{productId:guid}/units")]
    public async Task<IActionResult> GetUnitsAsync([FromRoute] Guid productId, CancellationToken ct)
        => Ok(await gateway.CallAsync("Products.GetUnitsAsync", (sdk, token) => sdk.Products.GetUnitsAsync(productId, token), id: productId.ToString(), context: null, ct: ct));

    [HttpPost("products")]
    public async Task<IActionResult> CreateAsync([FromBody] ProductCreateRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("Products.CreateAsync", (sdk, token) => sdk.Products.CreateAsync(request, token), id: null, context: null, ct: ct));

    [HttpPut("products/{productId:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid productId, [FromBody] ProductUpdateRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("Products.UpdateAsync", (sdk, token) => sdk.Products.UpdateAsync(productId, request, token), id: productId.ToString(), context: null, ct: ct));

    [HttpPost("products/{productId:guid}/delete")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid productId, CancellationToken ct)
        => Ok(await gateway.VoidAsync("Products.DeleteAsync", (sdk, token) => sdk.Products.DeleteAsync(productId, token), id: productId.ToString(), ct: ct));
}
