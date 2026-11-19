using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class LookupsController(SdkGateway gateway) : ControllerBase
{
    [HttpGet("lookups/branches")]
    public async Task<IActionResult> GetBranchesAsync(CancellationToken ct)
        => Ok(await gateway.CallAsync("Lookups.GetBranchesAsync", (sdk, token) => sdk.Lookups.GetBranchesAsync(token), id: null, context: null, ct: ct));

    [HttpGet("lookups/membership-ranks")]
    public async Task<IActionResult> GetMembershipRanksAsync(CancellationToken ct)
        => Ok(await gateway.CallAsync("Lookups.GetMembershipRanksAsync", (sdk, token) => sdk.Lookups.GetMembershipRanksAsync(token), id: null, context: null, ct: ct));
}
