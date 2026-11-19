using System.Text.Json.Serialization;
using SofiPOS.FnB.Http;
using SofiPOS.FnB.Models;

namespace SofiPOS.FnB.Services;

/// <summary>
/// Dữ liệu phục vụ bán hàng FnB (<c>/fnb</c>): khu vực/bàn, menu, món kèm, thuộc tính, phương thức thanh toán.
/// Tham số <c>branchId</c> bỏ trống thì dùng chi nhánh mặc định của đối tác; chi nhánh phải là mô hình FnB và đang hoạt động.
/// </summary>
public sealed class FnbService : ServiceBase
{
    internal FnbService(ApiClient api) : base(api)
    {
    }

    /// <summary>Khu vực, bàn, đơn đang dùng bàn và lịch đặt bàn sắp tới. <c>GET /fnb/areas</c></summary>
    public Task<List<FnbArea>> GetAreasAsync(Guid? branchId = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<List<FnbArea>>("fnb/areas", new QueryString().Add("branchId", branchId), cancellationToken);

    /// <summary>
    /// Menu bán theo chi nhánh/kênh bán: món đang bán kèm giá, giá flash sale, combo đang bán, nhóm Menu. <c>GET /fnb/menu</c>
    /// </summary>
    /// <param name="branchId">Chi nhánh; bỏ trống dùng chi nhánh mặc định.</param>
    /// <param name="saleChannelId">Kênh bán, ví dụ <c>TaiCho</c>; bỏ trống dùng mặc định của server.</param>
    /// <param name="cancellationToken">Token hủy.</param>
    public Task<FnbMenu> GetMenuAsync(Guid? branchId = null, string? saleChannelId = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<FnbMenu>("fnb/menu", new QueryString().Add("branchId", branchId).Add("saleChannelId", saleChannelId), cancellationToken);

    /// <summary>Món kèm được chọn cho món và giá món kèm. <c>GET /fnb/products/{productId}/addons</c></summary>
    public Task<List<FnbMenuProduct>> GetProductAddOnsAsync(Guid productId, Guid? branchId = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<List<FnbMenuProduct>>($"fnb/products/{Id(productId)}/addons", new QueryString().Add("branchId", branchId), cancellationToken);

    /// <summary>Thuộc tính của món (<c>Id</c>, <c>Title</c>, <c>Type</c>, <c>Values</c>). <c>GET /fnb/products/{productId}/attributes</c></summary>
    public Task<List<ProductAttribute>> GetProductAttributesAsync(Guid productId, Guid? branchId = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<List<ProductAttribute>>($"fnb/products/{Id(productId)}/attributes", new QueryString().Add("branchId", branchId), cancellationToken);

    /// <summary>Phương thức thanh toán của chi nhánh. <c>GET /fnb/payment-options</c></summary>
    public Task<List<PaymentOption>> GetPaymentOptionsAsync(Guid? branchId = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<List<PaymentOption>>("fnb/payment-options", new QueryString().Add("branchId", branchId), cancellationToken);
}

/// <summary>
/// Đơn hàng FnB (<c>/fnb/orders</c>): tạo, đọc, sửa đơn, món, combo, chuyển/gộp/tách bàn.
/// Thao tác trên đơn trả chi tiết đơn sau khi lưu. Đơn đã hoàn tất hoặc đã hủy không sửa được (422).
/// </summary>
public sealed class FnbOrdersService : ServiceBase
{
    internal FnbOrdersService(ApiClient api) : base(api)
    {
    }

    /// <summary>
    /// Tạo đơn. Đặt <see cref="FnbOrderCreateRequest.RequestId"/> để gọi lại an toàn khi mất kết nối:
    /// gọi lại cùng <c>requestId</c> trả về đơn đã tạo. SDK không tự gửi lại request này. <c>POST /fnb/orders</c>
    /// </summary>
    public Task<FnbOrder> CreateAsync(FnbOrderCreateRequest request, Guid? branchId = null, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>("fnb/orders", ProductsService.Required(request), cancellationToken, new QueryString().Add("branchId", branchId));

    /// <summary>Danh sách đơn của chi nhánh. <c>GET /fnb/orders</c></summary>
    public Task<PagedResult<FnbOrderSummary>> ListAsync(FnbOrderListQuery? query = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<PagedResult<FnbOrderSummary>>("fnb/orders", Page(query)
            .Add("branchId", query?.BranchId).Add("fromDate", query?.FromDate).Add("toDate", query?.ToDate)
            .Add("isPaid", query?.IsPaid).Add("orderCode", query?.OrderCode).Add("keyWord", query?.KeyWord), cancellationToken);

    /// <summary>Duyệt toàn bộ đơn qua mọi trang.</summary>
    public IAsyncEnumerable<FnbOrderSummary> ListAllAsync(FnbOrderListQuery? query = null, CancellationToken cancellationToken = default)
        => EnumerateAsync(query ?? new FnbOrderListQuery(), ListAsync, cancellationToken);

    /// <summary>Chi tiết đơn: món/combo (kèm <c>Id</c> dòng), khách, tổng tiền, thanh toán. <c>GET /fnb/orders/{orderId}</c></summary>
    public Task<FnbOrder> GetAsync(Guid orderId, CancellationToken cancellationToken = default)
        => Api.GetAsync<FnbOrder>($"fnb/orders/{Id(orderId)}", null, cancellationToken);

    /// <summary>Sửa ghi chú đơn. <c>PUT /fnb/orders/{orderId}/note</c></summary>
    public Task<FnbOrder> UpdateNoteAsync(Guid orderId, string? note, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/note", new NoteBody(note), cancellationToken);

    /// <summary>Gán/đổi khách; <paramref name="customerId"/> <c>null</c> để bỏ khách. <c>PUT /fnb/orders/{orderId}/customer</c></summary>
    public Task<FnbOrder> SetCustomerAsync(Guid orderId, Guid? customerId, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/customer", new CustomerBody(customerId), cancellationToken);

    /// <summary>Hủy đơn (đơn đã hoàn tất không hủy được). <c>POST /fnb/orders/{orderId}/cancel</c></summary>
    public Task<FnbOrder> CancelAsync(Guid orderId, string cancelReason, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/cancel", new CancelOrderBody(cancelReason), cancellationToken);

    /// <summary>Thêm một món. <c>POST /fnb/orders/{orderId}/products</c></summary>
    public Task<FnbOrder> AddProductAsync(Guid orderId, FnbOrderItemRequest item, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/products", ProductsService.Required(item), cancellationToken);

    /// <summary>Thêm nhiều món. <c>POST /fnb/orders/{orderId}/products/bulk</c></summary>
    public Task<FnbOrder> AddProductsAsync(Guid orderId, IEnumerable<FnbOrderItemRequest> items, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/products/bulk", (items ?? throw new ArgumentNullException(nameof(items))).ToList(), cancellationToken);

    /// <summary>Đổi số lượng dòng món. <c>PUT /fnb/orders/{orderId}/products/{lineId}/quantity</c></summary>
    public Task<FnbOrder> UpdateProductQuantityAsync(Guid orderId, Guid lineId, decimal quantity, string? reason = null, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/quantity", new QuantityBody(quantity, reason), cancellationToken);

    /// <summary>Đổi đơn vị tính dòng món (<c>null</c> là đơn vị cơ bản); giá do server tính. <c>PUT .../products/{lineId}/unit</c></summary>
    public Task<FnbOrder> UpdateProductUnitAsync(Guid orderId, Guid lineId, Guid? unitId, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/unit", new UnitBody(unitId), cancellationToken);

    /// <summary>Đổi giá dòng món theo quy tắc điều chỉnh giá của cửa hàng. <c>PUT .../products/{lineId}/price</c></summary>
    public Task<FnbOrder> UpdateProductPriceAsync(Guid orderId, Guid lineId, decimal price, string? reason = null, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/price", new PriceBody(price, reason), cancellationToken);

    /// <summary>Sửa ghi chú dòng món. <c>PUT .../products/{lineId}/note</c></summary>
    public Task<FnbOrder> UpdateProductNoteAsync(Guid orderId, Guid lineId, string? note, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/note", new NoteBody(note), cancellationToken);

    /// <summary>Thay thuộc tính dòng món. <c>PUT .../products/{lineId}/attributes</c></summary>
    public Task<FnbOrder> UpdateProductAttributesAsync(Guid orderId, Guid lineId, IEnumerable<AttributeValue> attributes, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/attributes", (attributes ?? throw new ArgumentNullException(nameof(attributes))).ToList(), cancellationToken);

    /// <summary>Thay toàn bộ món kèm của dòng món. <c>PUT .../products/{lineId}/addons</c></summary>
    public Task<FnbOrder> UpdateProductAddOnsAsync(Guid orderId, Guid lineId, IEnumerable<FnbOrderAddOnRequest> addOns, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/addons", new AddOnsBody((addOns ?? throw new ArgumentNullException(nameof(addOns))).ToList()), cancellationToken);

    /// <summary>Xóa dòng món chưa in chế biến. <c>POST .../products/{lineId}/delete</c></summary>
    public Task<FnbOrder> DeleteProductAsync(Guid orderId, Guid lineId, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"{Line(orderId, lineId)}/delete", null, cancellationToken);

    /// <summary>Hủy món đã in chế biến (0 &lt; số lượng ≤ số lượng hiện tại). <c>POST .../products/{lineId}/cancel</c></summary>
    public Task<FnbOrder> CancelProductAsync(Guid orderId, Guid lineId, decimal quantity, string reason, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"{Line(orderId, lineId)}/cancel", new CancelLineBody(quantity, reason), cancellationToken);

    /// <summary>Thêm một combo. <c>POST /fnb/orders/{orderId}/combos</c></summary>
    public Task<FnbOrder> AddComboAsync(Guid orderId, FnbOrderComboRequest combo, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/combos", ProductsService.Required(combo), cancellationToken);

    /// <summary>Thêm nhiều combo. <c>POST /fnb/orders/{orderId}/combos/bulk</c></summary>
    public Task<FnbOrder> AddCombosAsync(Guid orderId, IEnumerable<FnbOrderComboRequest> combos, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/combos/bulk", (combos ?? throw new ArgumentNullException(nameof(combos))).ToList(), cancellationToken);

    /// <summary>Sửa lựa chọn của dòng combo (combo giữ nguyên). <c>PUT /fnb/orders/{orderId}/combos/{lineId}</c></summary>
    public Task<FnbOrder> UpdateComboAsync(Guid orderId, Guid lineId, FnbOrderComboRequest combo, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>(ComboLine(orderId, lineId), ProductsService.Required(combo), cancellationToken);

    /// <summary>Xóa dòng combo chưa in chế biến. <c>POST .../combos/{lineId}/delete</c></summary>
    public Task<FnbOrder> DeleteComboAsync(Guid orderId, Guid lineId, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"{ComboLine(orderId, lineId)}/delete", null, cancellationToken);

    /// <summary>Hủy dòng combo. <c>POST .../combos/{lineId}/cancel</c></summary>
    public Task<FnbOrder> CancelComboAsync(Guid orderId, Guid lineId, string reason, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"{ComboLine(orderId, lineId)}/cancel", new ReasonBody(reason), cancellationToken);

    /// <summary>
    /// Chuyển đơn sang bàn khác và/hoặc đổi kênh bán. Truyền bàn hiện tại để chỉ đổi kênh;
    /// <paramref name="saleChannelId"/> <c>null</c> giữ kênh hiện tại. <c>PUT /fnb/orders/{orderId}/table-sale-channel</c>
    /// </summary>
    public Task<FnbOrder> ChangeTableAsync(Guid orderId, Guid? tableId, string? saleChannelId = null, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/table-sale-channel", new TableBody(tableId, saleChannelId), cancellationToken);

    /// <summary>Gộp đơn nguồn vào đơn đích (cùng chi nhánh, còn mở); trả chi tiết đơn đích. <c>POST /fnb/orders/merge</c></summary>
    public Task<FnbOrder> MergeAsync(Guid sourceOrderId, Guid targetOrderId, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>("fnb/orders/merge", new MergeBody(sourceOrderId, targetOrderId), cancellationToken);

    /// <summary>Chuyển một phần món/combo sang đơn khác hoặc tách sang đơn mới. <c>POST /fnb/orders/{orderId}/move-items</c></summary>
    public Task<MoveItemsResult> MoveItemsAsync(Guid orderId, FnbMoveItemsRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<MoveItemsResult>($"fnb/orders/{Id(orderId)}/move-items", ProductsService.Required(request), cancellationToken);

    private static string Line(Guid orderId, Guid lineId) => $"fnb/orders/{Id(orderId)}/products/{Id(lineId)}";

    private static string ComboLine(Guid orderId, Guid lineId) => $"fnb/orders/{Id(orderId)}/combos/{Id(lineId)}";

    private sealed record NoteBody([property: JsonPropertyName("note")] string? Note);

    private sealed record CustomerBody([property: JsonPropertyName("customerId"), JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? CustomerId);

    private sealed record CancelOrderBody([property: JsonPropertyName("cancelReason")] string CancelReason);

    private sealed record QuantityBody([property: JsonPropertyName("quantity")] decimal Quantity, [property: JsonPropertyName("reason")] string? Reason);

    private sealed record UnitBody([property: JsonPropertyName("unitId"), JsonIgnore(Condition = JsonIgnoreCondition.Never)] Guid? UnitId);

    private sealed record PriceBody([property: JsonPropertyName("price")] decimal Price, [property: JsonPropertyName("reason")] string? Reason);

    private sealed record AddOnsBody([property: JsonPropertyName("addOns")] List<FnbOrderAddOnRequest> AddOns);

    private sealed record CancelLineBody([property: JsonPropertyName("quantity")] decimal Quantity, [property: JsonPropertyName("reason")] string Reason);

    private sealed record ReasonBody([property: JsonPropertyName("reason")] string Reason);

    private sealed record TableBody([property: JsonPropertyName("tableId")] Guid? TableId, [property: JsonPropertyName("saleChannelId")] string? SaleChannelId);

    private sealed record MergeBody([property: JsonPropertyName("sourceOrderId")] Guid SourceOrderId, [property: JsonPropertyName("targetOrderId")] Guid TargetOrderId);
}

/// <summary>
/// Thanh toán đơn FnB: tạm tính, tạo QR, tra cứu giao dịch QR, ghi nhận thanh toán.
/// Ba bước tạm tính, tạo QR và thanh toán dùng chung quy tắc chiết khấu/phụ thu/voucher nên số tiền khớp nhau.
/// </summary>
public sealed class FnbPaymentsService : ServiceBase
{
    internal FnbPaymentsService(ApiClient api) : base(api)
    {
    }

    /// <summary>Tạm tính số tiền, không ghi nhận thanh toán. <c>POST /fnb/orders/{orderId}/payment/preview</c></summary>
    public Task<PaymentPreview> PreviewAsync(Guid orderId, FnbPaymentRequest? request = null, CancellationToken cancellationToken = default)
        => Api.PostAsync<PaymentPreview>($"fnb/orders/{Id(orderId)}/payment/preview", request ?? new FnbPaymentRequest(), cancellationToken);

    /// <summary>
    /// Tạo QR thanh toán theo nhà cung cấp. QR chỉ là yêu cầu thanh toán, đơn chỉ hoàn tất khi gọi <see cref="PayAsync"/>.
    /// <c>POST /fnb/orders/{orderId}/qr</c>
    /// </summary>
    public Task<PaymentQr> CreateQrAsync(Guid orderId, FnbPaymentQrRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<PaymentQr>($"fnb/orders/{Id(orderId)}/qr", ProductsService.Required(request), cancellationToken);

    /// <summary>
    /// Tra cứu giao dịch QR với nhà cung cấp có <c>SupportsTransactionCheck</c>.
    /// Trả <c>true</c> khi đã nhận tiền; chưa nhận tiền thì API trả 422 kèm thông báo (ném <see cref="Exceptions.SofiPosApiException"/>).
    /// <c>GET /fnb/orders/{orderId}/qr/check/{provider}</c>
    /// </summary>
    public Task<bool> CheckQrTransactionAsync(Guid orderId, string provider, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<bool>($"fnb/orders/{Id(orderId)}/qr/check/{Segment(provider)}", new QueryString().Add("transactionId", transactionId), cancellationToken);

    /// <summary>
    /// Ghi nhận thanh toán và hoàn tất đơn; gọi lại với đơn đã hoàn tất trả 422 (không ghi nhận hai lần).
    /// SDK không tự gửi lại request này. <c>PUT /fnb/orders/{orderId}/payment</c>
    /// </summary>
    public Task<FnbOrder> PayAsync(Guid orderId, FnbPaymentRequest request, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/payment", ProductsService.Required(request), cancellationToken);
}
