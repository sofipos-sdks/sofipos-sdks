using Microsoft.AspNetCore.Mvc;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo.Controllers;

[ApiController]
[Route("api/pos")]
public sealed class FnbOrdersController(SdkGateway gateway) : ControllerBase
{
    [HttpPost("fnb/orders")]
    public async Task<IActionResult> CreateAsync([FromQuery] Guid? branchId, [FromBody] FnbOrderCreateRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.CreateAsync", (sdk, token) => sdk.FnbOrders.CreateAsync(request, branchId, token), id: null, context: null, ct: ct));

    [HttpGet("fnb/orders")]
    public async Task<IActionResult> ListAsync([FromQuery] FnbOrderListQuery? query, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.ListAsync", (sdk, token) => sdk.FnbOrders.ListAsync(query, token), id: null, context: null, ct: ct));

    [HttpGet("fnb/orders/{orderId:guid}")]
    public async Task<IActionResult> GetAsync([FromRoute] Guid orderId, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.GetAsync", (sdk, token) => sdk.FnbOrders.GetAsync(orderId, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPut("fnb/orders/{orderId:guid}/note")]
    public async Task<IActionResult> UpdateNoteAsync([FromRoute] Guid orderId, [FromBody] FnbOrdersUpdateNoteBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.UpdateNoteAsync", (sdk, token) => sdk.FnbOrders.UpdateNoteAsync(orderId, body.Note, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPut("fnb/orders/{orderId:guid}/customer")]
    public async Task<IActionResult> SetCustomerAsync([FromRoute] Guid orderId, [FromBody] FnbOrdersSetCustomerBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.SetCustomerAsync", (sdk, token) => sdk.FnbOrders.SetCustomerAsync(orderId, body.CustomerId, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/{orderId:guid}/cancel")]
    public async Task<IActionResult> CancelAsync([FromRoute] Guid orderId, [FromBody] FnbOrdersCancelBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.CancelAsync", (sdk, token) => sdk.FnbOrders.CancelAsync(orderId, body.CancelReason, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/{orderId:guid}/products")]
    public async Task<IActionResult> AddProductAsync([FromRoute] Guid orderId, [FromBody] FnbOrderItemRequest item, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.AddProductAsync", (sdk, token) => sdk.FnbOrders.AddProductAsync(orderId, item, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/{orderId:guid}/products/bulk")]
    public async Task<IActionResult> AddProductsAsync([FromRoute] Guid orderId, [FromBody] List<FnbOrderItemRequest> items, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.AddProductsAsync", (sdk, token) => sdk.FnbOrders.AddProductsAsync(orderId, items, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPut("fnb/orders/{orderId:guid}/products/{lineId:guid}/quantity")]
    public async Task<IActionResult> UpdateProductQuantityAsync([FromRoute] Guid orderId, [FromRoute] Guid lineId, [FromBody] FnbOrdersUpdateProductQuantityBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.UpdateProductQuantityAsync", (sdk, token) => sdk.FnbOrders.UpdateProductQuantityAsync(orderId, lineId, body.Quantity, body.Reason, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPut("fnb/orders/{orderId:guid}/products/{lineId:guid}/unit")]
    public async Task<IActionResult> UpdateProductUnitAsync([FromRoute] Guid orderId, [FromRoute] Guid lineId, [FromBody] FnbOrdersUpdateProductUnitBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.UpdateProductUnitAsync", (sdk, token) => sdk.FnbOrders.UpdateProductUnitAsync(orderId, lineId, body.UnitId, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPut("fnb/orders/{orderId:guid}/products/{lineId:guid}/price")]
    public async Task<IActionResult> UpdateProductPriceAsync([FromRoute] Guid orderId, [FromRoute] Guid lineId, [FromBody] FnbOrdersUpdateProductPriceBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.UpdateProductPriceAsync", (sdk, token) => sdk.FnbOrders.UpdateProductPriceAsync(orderId, lineId, body.Price, body.Reason, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPut("fnb/orders/{orderId:guid}/products/{lineId:guid}/note")]
    public async Task<IActionResult> UpdateProductNoteAsync([FromRoute] Guid orderId, [FromRoute] Guid lineId, [FromBody] FnbOrdersUpdateProductNoteBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.UpdateProductNoteAsync", (sdk, token) => sdk.FnbOrders.UpdateProductNoteAsync(orderId, lineId, body.Note, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPut("fnb/orders/{orderId:guid}/products/{lineId:guid}/attributes")]
    public async Task<IActionResult> UpdateProductAttributesAsync([FromRoute] Guid orderId, [FromRoute] Guid lineId, [FromBody] List<AttributeValue> attributes, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.UpdateProductAttributesAsync", (sdk, token) => sdk.FnbOrders.UpdateProductAttributesAsync(orderId, lineId, attributes, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPut("fnb/orders/{orderId:guid}/products/{lineId:guid}/addons")]
    public async Task<IActionResult> UpdateProductAddOnsAsync([FromRoute] Guid orderId, [FromRoute] Guid lineId, [FromBody] List<FnbOrderAddOnRequest> addOns, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.UpdateProductAddOnsAsync", (sdk, token) => sdk.FnbOrders.UpdateProductAddOnsAsync(orderId, lineId, addOns, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/{orderId:guid}/products/{lineId:guid}/delete")]
    public async Task<IActionResult> DeleteProductAsync([FromRoute] Guid orderId, [FromRoute] Guid lineId, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.DeleteProductAsync", (sdk, token) => sdk.FnbOrders.DeleteProductAsync(orderId, lineId, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/{orderId:guid}/products/{lineId:guid}/cancel")]
    public async Task<IActionResult> CancelProductAsync([FromRoute] Guid orderId, [FromRoute] Guid lineId, [FromBody] FnbOrdersCancelProductBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.CancelProductAsync", (sdk, token) => sdk.FnbOrders.CancelProductAsync(orderId, lineId, body.Quantity, body.Reason, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/{orderId:guid}/combos")]
    public async Task<IActionResult> AddComboAsync([FromRoute] Guid orderId, [FromBody] FnbOrderComboRequest combo, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.AddComboAsync", (sdk, token) => sdk.FnbOrders.AddComboAsync(orderId, combo, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/{orderId:guid}/combos/bulk")]
    public async Task<IActionResult> AddCombosAsync([FromRoute] Guid orderId, [FromBody] List<FnbOrderComboRequest> combos, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.AddCombosAsync", (sdk, token) => sdk.FnbOrders.AddCombosAsync(orderId, combos, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPut("fnb/orders/{orderId:guid}/combos/{lineId:guid}")]
    public async Task<IActionResult> UpdateComboAsync([FromRoute] Guid orderId, [FromRoute] Guid lineId, [FromBody] FnbOrderComboRequest combo, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.UpdateComboAsync", (sdk, token) => sdk.FnbOrders.UpdateComboAsync(orderId, lineId, combo, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/{orderId:guid}/combos/{lineId:guid}/delete")]
    public async Task<IActionResult> DeleteComboAsync([FromRoute] Guid orderId, [FromRoute] Guid lineId, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.DeleteComboAsync", (sdk, token) => sdk.FnbOrders.DeleteComboAsync(orderId, lineId, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/{orderId:guid}/combos/{lineId:guid}/cancel")]
    public async Task<IActionResult> CancelComboAsync([FromRoute] Guid orderId, [FromRoute] Guid lineId, [FromBody] FnbOrdersCancelComboBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.CancelComboAsync", (sdk, token) => sdk.FnbOrders.CancelComboAsync(orderId, lineId, body.Reason, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPut("fnb/orders/{orderId:guid}/table-sale-channel")]
    public async Task<IActionResult> ChangeTableAsync([FromRoute] Guid orderId, [FromBody] FnbOrdersChangeTableBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.ChangeTableAsync", (sdk, token) => sdk.FnbOrders.ChangeTableAsync(orderId, body.TableId, body.SaleChannelId, token), id: orderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/merge")]
    public async Task<IActionResult> MergeAsync([FromBody] FnbOrdersMergeBody body, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.MergeAsync", (sdk, token) => sdk.FnbOrders.MergeAsync(body.SourceOrderId, body.TargetOrderId, token), id: body.SourceOrderId.ToString(), context: null, ct: ct));

    [HttpPost("fnb/orders/{orderId:guid}/move-items")]
    public async Task<IActionResult> MoveItemsAsync([FromRoute] Guid orderId, [FromBody] FnbMoveItemsRequest request, CancellationToken ct)
        => Ok(await gateway.CallAsync("FnbOrders.MoveItemsAsync", (sdk, token) => sdk.FnbOrders.MoveItemsAsync(orderId, request, token), id: orderId.ToString(), context: null, ct: ct));
}
