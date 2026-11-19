using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class PricingPoliciesController(SdkGateway gateway) : ControllerBase
{
    [HttpGet("pricing-policies")]
    public async Task<IActionResult> ListAsync([FromQuery] PricingPolicyListQuery? query, CancellationToken ct)
        => Ok(await gateway.CallAsync("PricingPolicies.ListAsync", (sdk, token) => sdk.PricingPolicies.ListAsync(query, token), id: null, context: null, ct: ct));

    [HttpGet("pricing-policies/{id:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.CallAsync("PricingPolicies.GetAsync", (sdk, token) => sdk.PricingPolicies.GetAsync(id, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("pricing-policies/product-addons")]
    public async Task<IActionResult> GetProductAddOnsAsync([FromBody] PricingPoliciesGetProductAddOnsBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("PricingPolicies.GetProductAddOnsAsync", (sdk, token) => sdk.PricingPolicies.GetProductAddOnsAsync(body.ProductIds, token), id: null, context: null, ct: ct));

    [HttpPost("pricing-policies")]
    public async Task<IActionResult> CreateAsync([FromBody] PricingPolicyRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("PricingPolicies.CreateAsync", (sdk, token) => sdk.PricingPolicies.CreateAsync(request, token), id: null, context: null, ct: ct));

    [HttpPut("pricing-policies/{id:guid}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] Guid id, [FromBody] PricingPolicyRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("PricingPolicies.UpdateAsync", (sdk, token) => sdk.PricingPolicies.UpdateAsync(id, request, token), id: id.ToString(), context: null, ct: ct));

    [HttpPost("pricing-policies/{id:guid}/delete")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
        => Ok(await gateway.VoidAsync("PricingPolicies.DeleteAsync", (sdk, token) => sdk.PricingPolicies.DeleteAsync(id, token), id: id.ToString(), ct: ct));
}
