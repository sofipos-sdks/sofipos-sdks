using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class FnbPaymentsController(SdkGateway gateway) : ControllerBase
{
    [HttpPost("fnb/orders/{orderId:guid}/payment/preview")]
    public async Task<IActionResult> PreviewAsync([FromQuery] Guid branchId, [FromRoute] Guid orderId, [FromBody] FnbPaymentRequest? request, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbPayments.PreviewAsync", (sdk, token) => sdk.FnbPayments.PreviewAsync(branchId, orderId, request, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/{orderId:guid}/qr")]
    public async Task<IActionResult> CreateQrAsync([FromQuery] Guid branchId, [FromRoute] Guid orderId, [FromBody] FnbPaymentQrRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbPayments.CreateQrAsync", (sdk, token) => sdk.FnbPayments.CreateQrAsync(branchId, orderId, request, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpGet("fnb/orders/{orderId:guid}/qr/check/{provider}")]
    public async Task<IActionResult> CheckQrTransactionAsync([FromQuery] Guid branchId, [FromRoute] Guid orderId, [FromRoute] string provider, [FromQuery] Guid? transactionId, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbPayments.CheckQrTransactionAsync", (sdk, token) => sdk.FnbPayments.CheckQrTransactionAsync(branchId, orderId, provider, transactionId, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPut("fnb/orders/{orderId:guid}/payment")]
    public async Task<IActionResult> PayAsync([FromQuery] Guid branchId, [FromRoute] Guid orderId, [FromBody] FnbPaymentRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbPayments.PayAsync", (sdk, token) => sdk.FnbPayments.PayAsync(branchId, orderId, request, token), id: orderId.ToString(), context: null, ct: ct));
}
