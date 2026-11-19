using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/connection")]
public sealed class ConnectionController(ConnectionService connections) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) => Ok(ConnectionService.View(await connections.GetAsync(ct)));

    [HttpPut]
    public async Task<IActionResult> Save([FromBody] ConnectionInput input, CancellationToken ct)
        => Ok(ConnectionService.View(await connections.SaveAsync(input, ct)));

    [HttpPost("check")]
    public async Task<IActionResult> Check([FromBody] ConnectionInput input, CancellationToken ct)
        => Ok(await connections.ProbeAsync(input, ct));
}
