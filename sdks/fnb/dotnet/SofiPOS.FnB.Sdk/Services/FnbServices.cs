using System.Text.Json.Serialization;
using SofiPOS.FnB.Http;
using SofiPOS.FnB.Models;

namespace SofiPOS.FnB.Services;

/// <summary>
/// Dữ liệu phục vụ bán hàng FnB (<c>/fnb</c>): khu vực/bàn, menu, món kèm, thuộc tính, phương thức thanh toán.
/// Mọi hàm bắt buộc nhận <c>branchId</c> (lấy từ <c>Lookups.GetBranchesAsync</c>), SDK gửi qua query <c>branchId</c>;
/// chi nhánh phải thuộc cửa hàng, đang hoạt động và là mô hình FnB.
/// </summary>
public sealed class FnbService : ServiceBase
{
    internal FnbService(ApiClient api) : base(api)
    {
    }

    /// <summary>Khu vực, bàn, đơn đang dùng bàn và lịch đặt bàn sắp tới. <c>GET /fnb/areas</c></summary>
    public Task<List<FnbArea>> GetAreasAsync(Guid branchId, CancellationToken cancellationToken = default)
        => Api.GetAsync<List<FnbArea>>("fnb/areas", Branch(branchId), cancellationToken);

    /// <summary>
    /// Menu bán theo chi nhánh/kênh bán: món đang bán kèm giá, giá flash sale, combo đang bán, nhóm Menu. <c>GET /fnb/menu</c>
    /// </summary>
    /// <param name="branchId">Chi nhánh FnB (bắt buộc).</param>
    /// <param name="saleChannelId">Kênh bán, ví dụ <c>TaiCho</c>; bỏ trống dùng mặc định của server.</param>
    /// <param name="cancellationToken">Token hủy.</param>
    public Task<FnbMenu> GetMenuAsync(Guid branchId, string? saleChannelId = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<FnbMenu>("fnb/menu", Branch(branchId).Add("saleChannelId", saleChannelId), cancellationToken);

    /// <summary>Món kèm được chọn cho món và giá món kèm. <c>GET /fnb/products/{productId}/addons</c></summary>
    public Task<List<FnbMenuProduct>> GetProductAddOnsAsync(Guid branchId, Guid productId, CancellationToken cancellationToken = default)
        => Api.GetAsync<List<FnbMenuProduct>>($"fnb/products/{Id(productId)}/addons", Branch(branchId), cancellationToken);

    /// <summary>Thuộc tính của món (<c>Id</c>, <c>Title</c>, <c>Type</c>, <c>Values</c>). <c>GET /fnb/products/{productId}/attributes</c></summary>
    public Task<List<ProductAttribute>> GetProductAttributesAsync(Guid branchId, Guid productId, CancellationToken cancellationToken = default)
        => Api.GetAsync<List<ProductAttribute>>($"fnb/products/{Id(productId)}/attributes", Branch(branchId), cancellationToken);

    /// <summary>Phương thức thanh toán của chi nhánh. <c>GET /fnb/payment-options</c></summary>
    public Task<List<PaymentOption>> GetPaymentOptionsAsync(Guid branchId, CancellationToken cancellationToken = default)
        => Api.GetAsync<List<PaymentOption>>("fnb/payment-options", Branch(branchId), cancellationToken);
}

/// <summary>
/// Đơn hàng FnB (<c>/fnb/orders</c>): tạo, đọc, sửa đơn, món, combo, chuyển/gộp/tách bàn.
/// Mọi hàm bắt buộc nhận <c>branchId</c>; đơn (kể cả đơn nguồn/đích khi gộp, chuyển món) phải thuộc chi nhánh này,
/// server không tự chuyển sang chi nhánh của đơn.
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
    public Task<FnbOrder> CreateAsync(Guid branchId, FnbOrderCreateRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>("fnb/orders", ProductsService.Required(request), cancellationToken, Branch(branchId));

    /// <summary>Danh sách đơn của chi nhánh (<see cref="FnbOrderListQuery.BranchId"/> bắt buộc). <c>GET /fnb/orders</c></summary>
    public Task<PagedResult<FnbOrderSummary>> ListAsync(FnbOrderListQuery query, CancellationToken cancellationToken = default)
    {
        if (query == null) throw new ArgumentNullException(nameof(query));
        return Api.GetAsync<PagedResult<FnbOrderSummary>>("fnb/orders", Page(query)
            .Add("branchId", RequiredBranch(query.BranchId)).Add("fromDate", query.FromDate).Add("toDate", query.ToDate)
            .Add("isPaid", query.IsPaid).Add("orderCode", query.OrderCode).Add("keyWord", query.KeyWord), cancellationToken);
    }

    /// <summary>Duyệt toàn bộ đơn của chi nhánh qua mọi trang.</summary>
    public IAsyncEnumerable<FnbOrderSummary> ListAllAsync(FnbOrderListQuery query, CancellationToken cancellationToken = default)
        => EnumerateAsync(query ?? throw new ArgumentNullException(nameof(query)), ListAsync, cancellationToken);

    /// <summary>Chi tiết đơn: món/combo (kèm <c>Id</c> dòng), khách, tổng tiền, thanh toán. <c>GET /fnb/orders/{orderId}</c></summary>
    public Task<FnbOrder> GetAsync(Guid branchId, Guid orderId, CancellationToken cancellationToken = default)
        => Api.GetAsync<FnbOrder>($"fnb/orders/{Id(orderId)}", Branch(branchId), cancellationToken);

    /// <summary>Sửa ghi chú đơn. <c>PUT /fnb/orders/{orderId}/note</c></summary>
    public Task<FnbOrder> UpdateNoteAsync(Guid branchId, Guid orderId, string? note, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/note", new NoteBody(note), cancellationToken, Branch(branchId));

    /// <summary>Gán/đổi khách; <paramref name="customerId"/> <c>null</c> để bỏ khách. <c>PUT /fnb/orders/{orderId}/customer</c></summary>
    public Task<FnbOrder> SetCustomerAsync(Guid branchId, Guid orderId, Guid? customerId, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/customer", new CustomerBody(customerId), cancellationToken, Branch(branchId));

    /// <summary>Hủy đơn (đơn đã hoàn tất không hủy được). <c>POST /fnb/orders/{orderId}/cancel</c></summary>
    public Task<FnbOrder> CancelAsync(Guid branchId, Guid orderId, string cancelReason, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/cancel", new CancelOrderBody(cancelReason), cancellationToken, Branch(branchId));

    /// <summary>Thêm một món. <c>POST /fnb/orders/{orderId}/products</c></summary>
    public Task<FnbOrder> AddProductAsync(Guid branchId, Guid orderId, FnbOrderItemRequest item, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/products", ProductsService.Required(item), cancellationToken, Branch(branchId));

    /// <summary>Thêm nhiều món. <c>POST /fnb/orders/{orderId}/products/bulk</c></summary>
    public Task<FnbOrder> AddProductsAsync(Guid branchId, Guid orderId, IEnumerable<FnbOrderItemRequest> items, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/products/bulk", (items ?? throw new ArgumentNullException(nameof(items))).ToList(), cancellationToken, Branch(branchId));

    /// <summary>Đổi số lượng dòng món. <c>PUT /fnb/orders/{orderId}/products/{lineId}/quantity</c></summary>
    public Task<FnbOrder> UpdateProductQuantityAsync(Guid branchId, Guid orderId, Guid lineId, decimal quantity, string? reason = null, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/quantity", new QuantityBody(quantity, reason), cancellationToken, Branch(branchId));

    /// <summary>Đổi đơn vị tính dòng món (<c>null</c> là đơn vị cơ bản); giá do server tính. <c>PUT .../products/{lineId}/unit</c></summary>
    public Task<FnbOrder> UpdateProductUnitAsync(Guid branchId, Guid orderId, Guid lineId, Guid? unitId, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/unit", new UnitBody(unitId), cancellationToken, Branch(branchId));

    /// <summary>Đổi giá dòng món theo quy tắc điều chỉnh giá của cửa hàng. <c>PUT .../products/{lineId}/price</c></summary>
    public Task<FnbOrder> UpdateProductPriceAsync(Guid branchId, Guid orderId, Guid lineId, decimal price, string? reason = null, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/price", new PriceBody(price, reason), cancellationToken, Branch(branchId));

    /// <summary>Sửa ghi chú dòng món. <c>PUT .../products/{lineId}/note</c></summary>
    public Task<FnbOrder> UpdateProductNoteAsync(Guid branchId, Guid orderId, Guid lineId, string? note, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/note", new NoteBody(note), cancellationToken, Branch(branchId));

    /// <summary>Thay thuộc tính dòng món. <c>PUT .../products/{lineId}/attributes</c></summary>
    public Task<FnbOrder> UpdateProductAttributesAsync(Guid branchId, Guid orderId, Guid lineId, IEnumerable<AttributeValue> attributes, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/attributes", (attributes ?? throw new ArgumentNullException(nameof(attributes))).ToList(), cancellationToken, Branch(branchId));

    /// <summary>Thay toàn bộ món kèm của dòng món. <c>PUT .../products/{lineId}/addons</c></summary>
    public Task<FnbOrder> UpdateProductAddOnsAsync(Guid branchId, Guid orderId, Guid lineId, IEnumerable<FnbOrderAddOnRequest> addOns, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"{Line(orderId, lineId)}/addons", new AddOnsBody((addOns ?? throw new ArgumentNullException(nameof(addOns))).ToList()), cancellationToken, Branch(branchId));

    /// <summary>Xóa dòng món chưa in chế biến. <c>POST .../products/{lineId}/delete</c></summary>
    public Task<FnbOrder> DeleteProductAsync(Guid branchId, Guid orderId, Guid lineId, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"{Line(orderId, lineId)}/delete", null, cancellationToken, Branch(branchId));

    /// <summary>Hủy món đã in chế biến (0 &lt; số lượng ≤ số lượng hiện tại). <c>POST .../products/{lineId}/cancel</c></summary>
    public Task<FnbOrder> CancelProductAsync(Guid branchId, Guid orderId, Guid lineId, decimal quantity, string reason, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"{Line(orderId, lineId)}/cancel", new CancelLineBody(quantity, reason), cancellationToken, Branch(branchId));

    /// <summary>Thêm một combo. <c>POST /fnb/orders/{orderId}/combos</c></summary>
    public Task<FnbOrder> AddComboAsync(Guid branchId, Guid orderId, FnbOrderComboRequest combo, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/combos", ProductsService.Required(combo), cancellationToken, Branch(branchId));

    /// <summary>Thêm nhiều combo. <c>POST /fnb/orders/{orderId}/combos/bulk</c></summary>
    public Task<FnbOrder> AddCombosAsync(Guid branchId, Guid orderId, IEnumerable<FnbOrderComboRequest> combos, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/combos/bulk", (combos ?? throw new ArgumentNullException(nameof(combos))).ToList(), cancellationToken, Branch(branchId));

    /// <summary>Sửa lựa chọn của dòng combo (combo giữ nguyên). <c>PUT /fnb/orders/{orderId}/combos/{lineId}</c></summary>
    public Task<FnbOrder> UpdateComboAsync(Guid branchId, Guid orderId, Guid lineId, FnbOrderComboRequest combo, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>(ComboLine(orderId, lineId), ProductsService.Required(combo), cancellationToken, Branch(branchId));

    /// <summary>Xóa dòng combo chưa in chế biến. <c>POST .../combos/{lineId}/delete</c></summary>
    public Task<FnbOrder> DeleteComboAsync(Guid branchId, Guid orderId, Guid lineId, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"{ComboLine(orderId, lineId)}/delete", null, cancellationToken, Branch(branchId));

    /// <summary>Hủy dòng combo. <c>POST .../combos/{lineId}/cancel</c></summary>
    public Task<FnbOrder> CancelComboAsync(Guid branchId, Guid orderId, Guid lineId, string reason, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>($"{ComboLine(orderId, lineId)}/cancel", new ReasonBody(reason), cancellationToken, Branch(branchId));

    /// <summary>
    /// Chuyển đơn sang bàn khác và/hoặc đổi kênh bán. Truyền bàn hiện tại để chỉ đổi kênh;
    /// <paramref name="saleChannelId"/> <c>null</c> giữ kênh hiện tại. <c>PUT /fnb/orders/{orderId}/table-sale-channel</c>
    /// </summary>
    public Task<FnbOrder> ChangeTableAsync(Guid branchId, Guid orderId, Guid? tableId, string? saleChannelId = null, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/table-sale-channel", new TableBody(tableId, saleChannelId), cancellationToken, Branch(branchId));

    /// <summary>
    /// Gộp đơn nguồn vào đơn đích; cả hai phải thuộc <paramref name="branchId"/> và còn mở; trả chi tiết đơn đích.
    /// <c>POST /fnb/orders/merge</c>
    /// </summary>
    public Task<FnbOrder> MergeAsync(Guid branchId, Guid sourceOrderId, Guid targetOrderId, CancellationToken cancellationToken = default)
        => Api.PostAsync<FnbOrder>("fnb/orders/merge", new MergeBody(sourceOrderId, targetOrderId), cancellationToken, Branch(branchId));

    /// <summary>
    /// Chuyển một phần món/combo sang đơn khác (cùng chi nhánh) hoặc tách sang đơn mới. <c>POST /fnb/orders/{orderId}/move-items</c>
    /// </summary>
    public Task<MoveItemsResult> MoveItemsAsync(Guid branchId, Guid orderId, FnbMoveItemsRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<MoveItemsResult>($"fnb/orders/{Id(orderId)}/move-items", ProductsService.Required(request), cancellationToken, Branch(branchId));

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
/// Mọi hàm bắt buộc nhận <c>branchId</c>, đơn phải thuộc chi nhánh này.
/// Ba bước tạm tính, tạo QR và thanh toán dùng chung quy tắc chiết khấu/phụ thu/voucher nên số tiền khớp nhau.
/// </summary>
public sealed class FnbPaymentsService : ServiceBase
{
    internal FnbPaymentsService(ApiClient api) : base(api)
    {
    }

    /// <summary>Tạm tính số tiền, không ghi nhận thanh toán. <c>POST /fnb/orders/{orderId}/payment/preview</c></summary>
    public Task<PaymentPreview> PreviewAsync(Guid branchId, Guid orderId, FnbPaymentRequest? request = null, CancellationToken cancellationToken = default)
        => Api.PostAsync<PaymentPreview>($"fnb/orders/{Id(orderId)}/payment/preview", request ?? new FnbPaymentRequest(), cancellationToken, Branch(branchId));

    /// <summary>
    /// Tạo QR thanh toán theo nhà cung cấp. QR chỉ là yêu cầu thanh toán, đơn chỉ hoàn tất khi gọi <see cref="PayAsync"/>.
    /// <c>POST /fnb/orders/{orderId}/qr</c>
    /// </summary>
    public Task<PaymentQr> CreateQrAsync(Guid branchId, Guid orderId, FnbPaymentQrRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<PaymentQr>($"fnb/orders/{Id(orderId)}/qr", ProductsService.Required(request), cancellationToken, Branch(branchId));

    /// <summary>
    /// Tra cứu giao dịch QR với nhà cung cấp có <c>SupportsTransactionCheck</c>.
    /// Trả <c>true</c> khi đã nhận tiền; chưa nhận tiền thì API trả 422 kèm thông báo (ném <see cref="Exceptions.SofiPosApiException"/>).
    /// <c>GET /fnb/orders/{orderId}/qr/check/{provider}</c>
    /// </summary>
    public Task<bool> CheckQrTransactionAsync(Guid branchId, Guid orderId, string provider, Guid? transactionId = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<bool>($"fnb/orders/{Id(orderId)}/qr/check/{Segment(provider)}", Branch(branchId).Add("transactionId", transactionId), cancellationToken);

    /// <summary>
    /// Ghi nhận thanh toán và hoàn tất đơn; gọi lại với đơn đã hoàn tất trả 422 (không ghi nhận hai lần).
    /// SDK không tự gửi lại request này. <c>PUT /fnb/orders/{orderId}/payment</c>
    /// </summary>
    public Task<FnbOrder> PayAsync(Guid branchId, Guid orderId, FnbPaymentRequest request, CancellationToken cancellationToken = default)
        => Api.PutAsync<FnbOrder>($"fnb/orders/{Id(orderId)}/payment", ProductsService.Required(request), cancellationToken, Branch(branchId));
}
