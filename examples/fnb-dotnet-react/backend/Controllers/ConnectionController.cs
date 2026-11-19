using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/connection")]
public sealed class ConnectionController(ConnectionService connections, SdkGateway gateway) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(ConnectionService.View(await connections.GetAsync(ct)));

    [HttpPut]
    public async Task<IActionResult> Save([FromBody] ConnectionInput input, CancellationToken ct)
        => Ok(ConnectionService.View(await connections.SaveAsync(input, ct)));

    [HttpPost("check")]
    public async Task<IActionResult> Check(CancellationToken ct)
        => Ok(await gateway.CallAsync("Lookups.GetBranchesAsync", (sdk, token) => sdk.Lookups.GetBranchesAsync(token), ct: ct));
}
