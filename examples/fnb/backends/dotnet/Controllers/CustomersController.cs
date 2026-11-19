using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class CustomersController(SdkGateway gateway) : ControllerBase
{
    [HttpGet("customers")]
    public async Task<IActionResult> ListAsync([FromQuery] CustomerListQuery? query, CancellationToken ct)
        => Ok(await gateway.CallAsync("Customers.ListAsync", (sdk, token) => sdk.Customers.ListAsync(query, token), id: null, context: null, ct: ct));

    [HttpGet("customers/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.CallAsync("Customers.GetAsync", (sdk, token) => sdk.Customers.GetAsync(id, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("customers")]
    public async Task<IActionResult> CreateAsync([FromBody] CustomerRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("Customers.CreateAsync", (sdk, token) => sdk.Customers.CreateAsync(request, token), id: null, context: null, ct: ct));

    [HttpPut("customers/{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] CustomerRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("Customers.UpdateAsync", (sdk, token) => sdk.Customers.UpdateAsync(id, request, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("customers/{id:guid}/delete")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.VoidAsync("Customers.DeleteAsync", (sdk, token) => sdk.Customers.DeleteAsync(id, token), id: id.ToString(), ct: ct));
}
