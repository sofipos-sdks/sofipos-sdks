using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class CombosController(SdkGateway gateway) : ControllerBase
{
    [HttpGet("combos")]
    public async Task<IActionResult> ListAsync([FromQuery] ComboListQuery? query, CancellationToken ct)
        => Ok(await gateway.CallAsync("Combos.ListAsync", (sdk, token) => sdk.Combos.ListAsync(query, token), id: null, context: null, ct: ct));

    [HttpGet("combos/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.CallAsync("Combos.GetAsync", (sdk, token) => sdk.Combos.GetAsync(id, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("combos")]
    public async Task<IActionResult> CreateAsync([FromBody] ComboRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("Combos.CreateAsync", (sdk, token) => sdk.Combos.CreateAsync(request, token), id: null, context: null, ct: ct));

    [HttpPut("combos/{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] ComboRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("Combos.UpdateAsync", (sdk, token) => sdk.Combos.UpdateAsync(id, request, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("combos/{id:guid}/delete")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.VoidAsync("Combos.DeleteAsync", (sdk, token) => sdk.Combos.DeleteAsync(id, token), id: id.ToString(), ct: ct));
}
