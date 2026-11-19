using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Đơn FnB trong danh sách hoặc đơn đang dùng bàn.</summary>
public sealed class FnbOrderSummary
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Mã.</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>ID khách hàng.</summary>
    [JsonPropertyName("customerId")]
    public Guid? CustomerId { get; set; }

    /// <summary>Tên khách hàng.</summary>
    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    /// <summary>Số điện thoại khách hàng.</summary>
    [JsonPropertyName("customerPhone")]
    public string? CustomerPhone { get; set; }

    /// <summary>Địa chỉ khách hàng.</summary>
    [JsonPropertyName("customerAddress")]
    public string? CustomerAddress { get; set; }

    /// <summary>Số lượng món trong đơn.</summary>
    [JsonPropertyName("countProducts")]
    public int CountProducts { get; set; }

    /// <summary>Tổng tiền.</summary>
    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    /// <summary>Tên chi nhánh.</summary>
    [JsonPropertyName("branchName")]
    public string? BranchName { get; set; }

    /// <summary>Tiền đặt cọc.</summary>
    [JsonPropertyName("deposit")]
    public decimal Deposit { get; set; }

    /// <summary>ID chi nhánh.</summary>
    [JsonPropertyName("branchId")]
    public Guid BranchId { get; set; }

    /// <summary>Voucher áp dụng.</summary>
    [JsonPropertyName("voucher")]
    public OrderVoucher? Voucher { get; set; }

    /// <summary>Thanh toán bằng điểm thành viên.</summary>
    [JsonPropertyName("membership")]
    public OrderMembership? Membership { get; set; }

    /// <summary>Lỗi phát hành hóa đơn điện tử (nếu có).</summary>
    [JsonPropertyName("invoiceError")]
    public OrderInvoiceError? InvoiceError { get; set; }

    /// <summary>Lý do hủy.</summary>
    [JsonPropertyName("cancelReason")]
    public string? CancelReason { get; set; }

    /// <summary>Thời điểm hủy.</summary>
    [JsonPropertyName("cancelDate")]
    public DateTime? CancelDate { get; set; }

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime? CreateDate { get; set; }

    /// <summary>Thời điểm thanh toán.</summary>
    [JsonPropertyName("paymentDate")]
    public DateTime? PaymentDate { get; set; }

    /// <summary>Đường dẫn tra cứu hóa đơn điện tử.</summary>
    [JsonPropertyName("invoiceUrl")]
    public string InvoiceUrl { get; set; } = string.Empty;

    /// <summary>Danh sách thuộc tính.</summary>
    [JsonPropertyName("attributes")]
    public List<ProductAttribute> Attributes { get; set; } = new();

    /// <summary>ID kênh bán (ví dụ <c>TaiCho</c>).</summary>
    [JsonPropertyName("saleChannelId")]
    public string? SaleChannelId { get; set; }

    /// <summary>ID bàn.</summary>
    [JsonPropertyName("tableId")]
    public Guid? TableId { get; set; }

    /// <summary>Tên bàn.</summary>
    [JsonPropertyName("tableName")]
    public string? TableName { get; set; }

    /// <summary>ID lịch đặt bàn tạo ra đơn.</summary>
    [JsonPropertyName("sourceScheduleId")]
    public Guid? SourceScheduleId { get; set; }

    /// <summary>Đơn bán theo bàn.</summary>
    [JsonPropertyName("isTableOrder")]
    public bool IsTableOrder { get; set; }

    /// <summary>Nền tảng tạo đơn.</summary>
    [JsonPropertyName("platform")]
    public string? Platform { get; set; }

    /// <summary>Thời điểm xác nhận đơn.</summary>
    [JsonPropertyName("confirmDate")]
    public DateTime? ConfirmDate { get; set; }

    /// <summary>Đơn E-Menu còn món chờ xác nhận (ConfirmDate null hoặc có món CreateDate &gt; ConfirmDate). Set từ mapper khi list bàn; với detail dùng computed từ products.</summary>
    [JsonPropertyName("isPendingEMenuConfirm")]
    public bool IsPendingEMenuConfirm { get; set; }

    /// <summary>Thông tin khách nhập khi đơn tạo từ QR order.</summary>
    [JsonPropertyName("qrOrderInfo")]
    public OrderQrInfo? QrOrderInfo { get; set; }
}

/// <summary>Chi tiết đơn FnB.</summary>
public sealed class FnbOrder
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>ID kênh bán (ví dụ <c>TaiCho</c>).</summary>
    [JsonPropertyName("saleChannelId")]
    public string? SaleChannelId { get; set; }

    /// <summary>ID bàn.</summary>
    [JsonPropertyName("tableId")]
    public Guid? TableId { get; set; }

    /// <summary>Tên bàn.</summary>
    [JsonPropertyName("tableName")]
    public string? TableName { get; set; }

    /// <summary>Tên khu vực của bàn.</summary>
    [JsonPropertyName("areaName")]
    public string? AreaName { get; set; }

    /// <summary>Mã.</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>ID khách hàng.</summary>
    [JsonPropertyName("customerId")]
    public Guid? CustomerId { get; set; }

    /// <summary>Tên khách hàng.</summary>
    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    /// <summary>Số điện thoại khách hàng.</summary>
    [JsonPropertyName("customerPhone")]
    public string? CustomerPhone { get; set; }

    /// <summary>Địa chỉ khách hàng.</summary>
    [JsonPropertyName("customerAddress")]
    public string? CustomerAddress { get; set; }

    /// <summary>Lịch đặt nguồn (check-in); lưu tại Order.ImportFrom khi tạo đơn từ lịch.</summary>
    [JsonPropertyName("sourceScheduleId")]
    public Guid? SourceScheduleId { get; set; }

    /// <summary>Đơn tạo từ check-in lịch, chưa có món — mobile dùng để bỏ qua validate sản phẩm.</summary>
    [JsonPropertyName("allowEmptyOrder")]
    public bool AllowEmptyOrder { get; set; }

    /// <summary>ID người tạo.</summary>
    [JsonPropertyName("createById")]
    public Guid? CreateById { get; set; }

    /// <summary>Tên người tạo.</summary>
    [JsonPropertyName("createByName")]
    public string? CreateByName { get; set; }

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }

    /// <summary>ID người thanh toán.</summary>
    [JsonPropertyName("paymentById")]
    public Guid? PaymentById { get; set; }

    /// <summary>Tên người thanh toán.</summary>
    [JsonPropertyName("paymentByName")]
    public string? PaymentByName { get; set; }

    /// <summary>Thời điểm thanh toán.</summary>
    [JsonPropertyName("paymentDate")]
    public DateTime? PaymentDate { get; set; }

    /// <summary>Thời điểm hủy.</summary>
    [JsonPropertyName("cancelDate")]
    public DateTime? CancelDate { get; set; }

    /// <summary>Lý do hủy.</summary>
    [JsonPropertyName("cancelReason")]
    public string? CancelReason { get; set; }

    /// <summary>Tiền đặt cọc.</summary>
    [JsonPropertyName("deposit")]
    public decimal Deposit { get; set; }

    /// <summary>Ghi chú đơn.</summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>Phương thức thanh toán.</summary>
    [JsonPropertyName("paymentMethod")]
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>Tổng phải thu (sau thuế, đã trừ voucher).</summary>
    [JsonPropertyName("totalAmountAfterTax")]
    public decimal TotalAmountAfterTax { get; set; }

    /// <summary>Tiền khách trả.</summary>
    [JsonPropertyName("customerPayment")]
    public decimal CustomerPayment { get; set; }

    /// <summary>Công nợ = TotalAmountAfterTax - CustomerPayment.</summary>
    [JsonPropertyName("debtAmount")]
    public decimal DebtAmount { get; set; }

    /// <summary>ID chi nhánh.</summary>
    [JsonPropertyName("branchId")]
    public Guid? BranchId { get; set; }

    /// <summary>Tên chi nhánh.</summary>
    [JsonPropertyName("branchName")]
    public string? BranchName { get; set; }

    /// <summary>Tổng điểm được nhận (tích điểm).</summary>
    [JsonPropertyName("membershipPointEarned")]
    public int MembershipPointEarned { get; set; }

    /// <summary>Voucher áp dụng.</summary>
    [JsonPropertyName("voucher")]
    public OrderVoucher? Voucher { get; set; }

    /// <summary>Thanh toán bằng điểm thành viên.</summary>
    [JsonPropertyName("membership")]
    public OrderMembership? Membership { get; set; }

    /// <summary>Giá trị giảm.</summary>
    [JsonPropertyName("discount")]
    public OrderDiscount? Discount { get; set; }

    /// <summary>Giá trị phụ thu.</summary>
    [JsonPropertyName("surcharge")]
    public OrderSurcharge? Surcharge { get; set; }

    /// <summary>Thông tin giao hàng.</summary>
    [JsonPropertyName("delivery")]
    public OrderDelivery? Delivery { get; set; }

    /// <summary>Danh sách sản phẩm.</summary>
    [JsonPropertyName("products")]
    public List<FnbOrderProduct> Products { get; set; } = new();

    /// <summary>Danh sách combo.</summary>
    [JsonPropertyName("combos")]
    public List<FnbOrderCombo> Combos { get; set; } = new();

    /// <summary>Danh sách thuộc tính.</summary>
    [JsonPropertyName("attributes")]
    public List<ProductAttribute>? Attributes { get; set; }

    /// <summary>Đường dẫn tra cứu hóa đơn điện tử.</summary>
    [JsonPropertyName("invoiceUrl")]
    public string InvoiceUrl { get; set; } = string.Empty;

    /// <summary>Lịch sử thanh toán (lần đầu + các lần trả nợ).</summary>
    [JsonPropertyName("paymentHistories")]
    public List<OrderPaymentHistory> PaymentHistories { get; set; } = new();

    /// <summary>Nền tảng tạo đơn.</summary>
    [JsonPropertyName("platform")]
    public string? Platform { get; set; }

    /// <summary>Thời điểm xác nhận đơn.</summary>
    [JsonPropertyName("confirmDate")]
    public DateTime? ConfirmDate { get; set; }

    /// <summary>Được gán từ OrderNotification khi API có ngữ cảnh E-Menu.</summary>
    [JsonPropertyName("isPendingEMenuConfirm")]
    public bool IsPendingEMenuConfirm { get; set; }

    /// <summary>Thông tin khách nhập khi đơn được tạo từ QR order.</summary>
    [JsonPropertyName("qrOrderInfo")]
    public OrderQrInfo? QrOrderInfo { get; set; }
}

/// <summary>Kết quả chuyển món/tách đơn.</summary>
public sealed class MoveItemsResult
{
    /// <summary>ID đơn nguồn.</summary>
    [JsonPropertyName("sourceOrderId")]
    public Guid SourceOrderId { get; set; }

    /// <summary>ID đơn nhận món.</summary>
    [JsonPropertyName("targetOrderId")]
    public Guid? TargetOrderId { get; set; }
}

/// <summary>Lỗi phát hành hóa đơn điện tử.</summary>
public sealed class OrderInvoiceError
{
    /// <summary>Thông báo.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

/// <summary>Thông tin mã QR đặt món tạo ra đơn.</summary>
public sealed class OrderQrInfo
{
    /// <summary>ID mã QR đặt món.</summary>
    [JsonPropertyName("qrOrderCodeId")]
    public Guid QrOrderCodeId { get; set; }

    /// <summary>Tên mã QR đặt món.</summary>
    [JsonPropertyName("qrName")]
    public string? QrName { get; set; }

    /// <summary>Tên khách hàng.</summary>
    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    /// <summary>Số điện thoại khách hàng.</summary>
    [JsonPropertyName("customerPhone")]
    public string? CustomerPhone { get; set; }
}

/// <summary>Thông tin giao hàng của đơn.</summary>
public sealed class OrderDelivery
{
    /// <summary>ID đơn hàng.</summary>
    [JsonPropertyName("orderId")]
    public Guid OrderId { get; set; }

    /// <summary>ID đơn vị giao hàng.</summary>
    [JsonPropertyName("deliveryId")]
    public Guid DeliveryId { get; set; }

    /// <summary>Tên đơn vị giao hàng.</summary>
    [JsonPropertyName("deliveryName")]
    public string? DeliveryName { get; set; }

    /// <summary>Mã đơn vị giao hàng.</summary>
    [JsonPropertyName("deliveryCode")]
    public string? DeliveryCode { get; set; }

    /// <summary>Phí giao hàng.</summary>
    [JsonPropertyName("deliveryFee")]
    public decimal DeliveryFee { get; set; }

    /// <summary>Số tiền thu hộ.</summary>
    [JsonPropertyName("cod")]
    public decimal COD { get; set; }

    /// <summary>Bên trả phí giao hàng.</summary>
    [JsonPropertyName("shippingFeePayer")]
    public ShippingFeePayer ShippingFeePayer { get; set; }

    /// <summary>Tên bên trả phí giao hàng.</summary>
    [JsonPropertyName("shippingFeePayerName")]
    public string? ShippingFeePayerName { get; set; }

    /// <summary>Ghi chú giao hàng.</summary>
    [JsonPropertyName("deliveryNote")]
    public string? DeliveryNote { get; set; }

    /// <summary>Tên người nhận.</summary>
    [JsonPropertyName("receiveName")]
    public string? ReceiveName { get; set; }

    /// <summary>Số điện thoại người nhận.</summary>
    [JsonPropertyName("receivePhone")]
    public string? ReceivePhone { get; set; }

    /// <summary>Địa chỉ nhận hàng.</summary>
    [JsonPropertyName("receiveAddress")]
    public string? ReceiveAddress { get; set; }

    /// <summary>Mã vận đơn.</summary>
    [JsonPropertyName("trackingCode")]
    public string? TrackingCode { get; set; }

    /// <summary>Trạng thái giao hàng.</summary>
    [JsonPropertyName("shippingStatus")]
    public string? ShippingStatus { get; set; }

    /// <summary>Mã trạng thái giao hàng.</summary>
    [JsonPropertyName("shippingStatusCode")]
    public int? ShippingStatusCode { get; set; }

    /// <summary>Có thể xác nhận đã nhận hàng.</summary>
    [JsonPropertyName("canConfirmReceived")]
    public bool CanConfirmReceived { get; set; }

    /// <summary>Ảnh lấy hàng.</summary>
    [JsonPropertyName("pickupImageUrls")]
    public List<string> PickupImageUrls { get; set; } = new();

    /// <summary>Ảnh giao hàng.</summary>
    [JsonPropertyName("deliveryImageUrls")]
    public List<string> DeliveryImageUrls { get; set; } = new();
}

/// <summary>Dòng món trong đơn. <c>Id</c> là lineId dùng cho các API sửa dòng.</summary>
public sealed class FnbOrderProduct
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>ID sản phẩm.</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>ID dòng món chính (với dòng món kèm).</summary>
    [JsonPropertyName("parentProductId")]
    public Guid? ParentProductId { get; set; }

    /// <summary>Mã sản phẩm.</summary>
    [JsonPropertyName("productCode")]
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>Số lượng.</summary>
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Loại.</summary>
    [JsonPropertyName("type")]
    public ProductType Type { get; set; }

    /// <summary>ID đơn vị tính; <c>null</c> hoặc GUID rỗng là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }

    /// <summary>Tên đơn vị tính.</summary>
    [JsonPropertyName("unitName")]
    public string? UnitName { get; set; }

    /// <summary>Thành tiền (chưa gồm VAT).</summary>
    [JsonPropertyName("totalCost")]
    public decimal TotalCost { get; set; }

    /// <summary>Thành tiền đã gồm VAT.</summary>
    [JsonPropertyName("totalCostIncludeVAT")]
    public decimal TotalCostIncludeVAT { get; set; }

    /// <summary>Giá đã gồm VAT.</summary>
    [JsonPropertyName("priceIncludeVAT")]
    public decimal PriceIncludeVAT { get; set; }

    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>ID nhóm sản phẩm.</summary>
    [JsonPropertyName("categoryId")]
    public Guid CategoryId { get; set; }

    /// <summary>Số thứ tự gọi món.</summary>
    [JsonPropertyName("numberOrder")]
    public decimal? NumberOrder { get; set; }

    /// <summary>Ghi chú.</summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>Có quản lý tồn kho.</summary>
    [JsonPropertyName("isInventory")]
    public bool IsInventory { get; set; }

    /// <summary>Thuộc diện giảm thuế GTGT.</summary>
    [JsonPropertyName("isTaxReductionApplicable")]
    public bool IsTaxReductionApplicable { get; set; }

    /// <summary>Đã in/gửi chế biến.</summary>
    [JsonPropertyName("isConfirm")]
    public bool IsConfirm { get; set; }

    /// <summary>Đã được chấp nhận (món gọi qua QR).</summary>
    [JsonPropertyName("isEMenuAccepted")]
    public bool IsEMenuAccepted { get; set; }

    /// <summary>Món do khách gọi qua QR.</summary>
    [JsonPropertyName("isFromEMenu")]
    public bool IsFromEMenu { get; set; }

    /// <summary>Trạng thái.</summary>
    [JsonPropertyName("status")]
    public OrderLineStatus Status { get; set; }

    /// <summary>Kiểu in của món.</summary>
    [JsonPropertyName("printType")]
    public ProductPrintType PrintType { get; set; }

    /// <summary>Danh sách thuộc tính.</summary>
    [JsonPropertyName("attributes")]
    public List<ProductAttribute> Attributes { get; set; } = new();

    /// <summary>Lịch sử đổi giá.</summary>
    [JsonPropertyName("histories")]
    public List<OrderProductPriceHistory> Histories { get; set; } = new();

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }

    /// <summary>Thời điểm hủy.</summary>
    [JsonPropertyName("cancelDate")]
    public DateTime? CancelDate { get; set; }

    /// <summary>Thời điểm in phiếu hủy.</summary>
    [JsonPropertyName("printCancelDate")]
    public DateTime? PrintCancelDate { get; set; }

    /// <summary>Lý do hủy.</summary>
    [JsonPropertyName("cancelReason")]
    public string? CancelReason { get; set; }

    /// <summary>Flash sale áp dụng cho dòng.</summary>
    [JsonPropertyName("flashSale")]
    public OrderProductFlashSale? FlashSale { get; set; }

    /// <summary>Chính sách giá áp dụng cho dòng.</summary>
    [JsonPropertyName("pricingPolicy")]
    public OrderProductPricingPolicy? PricingPolicy { get; set; }
}

/// <summary>Dòng combo trong đơn. <c>Id</c> là lineId dùng cho các API sửa dòng.</summary>
public sealed class FnbOrderCombo
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>ID đơn hàng.</summary>
    [JsonPropertyName("orderId")]
    public Guid OrderId { get; set; }

    /// <summary>ID combo trong danh mục.</summary>
    [JsonPropertyName("comboId")]
    public Guid ComboId { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>Số lượng.</summary>
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    /// <summary>Loại combo.</summary>
    [JsonPropertyName("comboType")]
    public ComboType ComboType { get; set; }

    /// <summary>Trạng thái.</summary>
    [JsonPropertyName("status")]
    public OrderLineStatus Status { get; set; }

    /// <summary>Cách tính giá combo.</summary>
    [JsonPropertyName("pricingMode")]
    public ComboPricingMode PricingMode { get; set; }

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Mã ngành nghề tính thuế.</summary>
    [JsonPropertyName("taxIndustryCategoryId")]
    public string? TaxIndustryCategoryId { get; set; }

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }

    /// <summary>Thời điểm hủy.</summary>
    [JsonPropertyName("cancelDate")]
    public DateTime? CancelDate { get; set; }

    /// <summary>Thời điểm in phiếu hủy.</summary>
    [JsonPropertyName("printCancelDate")]
    public DateTime? PrintCancelDate { get; set; }

    /// <summary>Lý do hủy.</summary>
    [JsonPropertyName("cancelReason")]
    public string? CancelReason { get; set; }

    /// <summary>Món do khách gọi qua QR.</summary>
    [JsonPropertyName("isFromEMenu")]
    public bool IsFromEMenu { get; set; }

    /// <summary>Xác định được nguồn tạo dòng.</summary>
    [JsonPropertyName("isSourceKnown")]
    public bool IsSourceKnown { get; set; }

    /// <summary>Số lượng gốc.</summary>
    [JsonPropertyName("originalQuantity")]
    public decimal? OriginalQuantity { get; set; }

    /// <summary>Giá gốc đã gồm VAT.</summary>
    [JsonPropertyName("originalPriceIncludeVAT")]
    public decimal? OriginalPriceIncludeVAT { get; set; }

    /// <summary>Lịch sử điều chỉnh của dòng (số lượng, giá) khi xác nhận đơn đặt qua QR.</summary>
    [JsonPropertyName("adjustments")]
    public List<FnbOrderComboAdjustment> Adjustments { get; set; } = new();

    /// <summary>Các nhóm.</summary>
    [JsonPropertyName("groups")]
    public List<FnbOrderComboGroup> Groups { get; set; } = new();

    /// <summary>Đã in/gửi chế biến.</summary>
    [JsonPropertyName("isConfirm")]
    public bool IsConfirm { get; set; }

    /// <summary>Đã được chấp nhận (món gọi qua QR).</summary>
    [JsonPropertyName("isEMenuAccepted")]
    public bool IsEMenuAccepted { get; set; }

    /// <summary>Đang chế biến.</summary>
    [JsonPropertyName("isProcessing")]
    public bool IsProcessing { get; set; }

    /// <summary>Giá combo đã gồm VAT.</summary>
    [JsonPropertyName("priceComboIncludeVAT")]
    public decimal PriceComboIncludeVAT { get; set; }

    /// <summary>Giá combo (chưa gồm VAT).</summary>
    [JsonPropertyName("priceCombo")]
    public decimal PriceCombo { get; set; }

    /// <summary>Toàn bộ sản phẩm thuộc combo (gộp từ các nhóm).</summary>
    [JsonPropertyName("allProducts")]
    public List<FnbOrderComboProduct> AllProducts { get; set; } = new();
}

/// <summary>Một lần thanh toán của đơn.</summary>
public sealed class OrderPaymentHistory
{
    /// <summary>Thời điểm.</summary>
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    /// <summary>Số tiền.</summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    /// <summary>"Thanh toán lần đầu" hoặc "Thanh toán nợ".</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Người thực hiện.</summary>
    [JsonPropertyName("byName")]
    public string ByName { get; set; } = string.Empty;

    /// <summary>Phương thức thanh toán (null = không xác định).</summary>
    [JsonPropertyName("paymentMethod")]
    public PaymentMethod? PaymentMethod { get; set; }
}

/// <summary>Lịch sử đổi giá của dòng món.</summary>
public sealed class OrderProductPriceHistory
{
    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createAt")]
    public DateTime CreateAt { get; set; }

    /// <summary>Giá cũ (chưa gồm VAT).</summary>
    [JsonPropertyName("oldPrice")]
    public decimal OldPrice { get; set; }

    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Giá cũ đã gồm VAT.</summary>
    [JsonPropertyName("oldPriceIncludeVAT")]
    public decimal OldPriceIncludeVAT { get; set; }

    /// <summary>Giá mới (chưa gồm VAT).</summary>
    [JsonPropertyName("newPrice")]
    public decimal NewPrice { get; set; }

    /// <summary>Giá mới đã gồm VAT.</summary>
    [JsonPropertyName("newPriceIncludeVAT")]
    public decimal NewPriceIncludeVAT { get; set; }

    /// <summary>ID người tạo.</summary>
    [JsonPropertyName("createBy")]
    public Guid CreateBy { get; set; }

    /// <summary>Tên người tạo.</summary>
    [JsonPropertyName("createByName")]
    public string? CreateByName { get; set; }
}

/// <summary>Flash sale áp dụng cho dòng món.</summary>
public sealed class OrderProductFlashSale
{
    /// <summary>ID sản phẩm trong flash sale.</summary>
    [JsonPropertyName("productFlashSaleId")]
    public Guid ProductFlashSaleId { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Ngày bắt đầu.</summary>
    [JsonPropertyName("startDate")]
    public DateTime? StartDate { get; set; }

    /// <summary>Ngày kết thúc; <c>null</c> là không giới hạn.</summary>
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }
}

/// <summary>Chính sách giá áp dụng cho dòng món.</summary>
public sealed class OrderProductPricingPolicy
{
    /// <summary>ID chính sách giá.</summary>
    [JsonPropertyName("pricingPolicyId")]
    public Guid PricingPolicyId { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Mốc chính sách hết áp dụng; null nếu không đổi trong 7 ngày tới.</summary>
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }
}

/// <summary>Điều chỉnh của dòng combo.</summary>
public sealed class FnbOrderComboAdjustment
{
    /// <summary>Loại.</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Số lượng trước điều chỉnh.</summary>
    [JsonPropertyName("previousQuantity")]
    public decimal? PreviousQuantity { get; set; }

    /// <summary>Số lượng.</summary>
    [JsonPropertyName("quantity")]
    public decimal? Quantity { get; set; }

    /// <summary>Giá trước điều chỉnh, đã gồm VAT.</summary>
    [JsonPropertyName("previousPriceIncludeVAT")]
    public decimal? PreviousPriceIncludeVAT { get; set; }

    /// <summary>Giá đã gồm VAT.</summary>
    [JsonPropertyName("priceIncludeVAT")]
    public decimal? PriceIncludeVAT { get; set; }

    /// <summary>Lý do.</summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }
}

/// <summary>Nhóm của dòng combo.</summary>
public sealed class FnbOrderComboGroup
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>ID nhóm.</summary>
    [JsonPropertyName("groupId")]
    public Guid GroupId { get; set; }

    /// <summary>ID dòng combo trong đơn.</summary>
    [JsonPropertyName("orderComboId")]
    public Guid OrderComboId { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Số món cần chọn trong nhóm.</summary>
    [JsonPropertyName("requiredQuantity")]
    public int RequiredQuantity { get; set; }

    /// <summary>Danh sách sản phẩm.</summary>
    [JsonPropertyName("products")]
    public List<FnbOrderComboProduct> Products { get; set; } = new();

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>Giá đã gồm VAT.</summary>
    [JsonPropertyName("priceIncludeVAT")]
    public decimal PriceIncludeVAT { get; set; }
}

/// <summary>Sản phẩm của dòng combo.</summary>
public sealed class FnbOrderComboProduct
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>ID nhóm.</summary>
    [JsonPropertyName("groupId")]
    public Guid? GroupId { get; set; }

    /// <summary>ID sản phẩm.</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>ID nhóm sản phẩm.</summary>
    [JsonPropertyName("categoryId")]
    public Guid CategoryId { get; set; }

    /// <summary>Có quản lý tồn kho.</summary>
    [JsonPropertyName("isInventory")]
    public bool IsInventory { get; set; }

    /// <summary>Số lượng.</summary>
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Mã.</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Thuế GTGT.</summary>
    [JsonPropertyName("vatgtgt")]
    public decimal VATGTGT { get; set; }

    /// <summary>Thuế TNCN.</summary>
    [JsonPropertyName("vattncn")]
    public decimal VATTNCN { get; set; }

    /// <summary>Tên đơn vị tính.</summary>
    [JsonPropertyName("unitName")]
    public string? UnitName { get; set; }

    /// <summary>ID đơn vị tính; <c>null</c> hoặc GUID rỗng là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }

    /// <summary>Đã in/gửi chế biến.</summary>
    [JsonPropertyName("isConfirm")]
    public bool IsConfirm { get; set; }

    /// <summary>Đã được chấp nhận (món gọi qua QR).</summary>
    [JsonPropertyName("isEMenuAccepted")]
    public bool IsEMenuAccepted { get; set; }

    /// <summary>Kiểu in của món.</summary>
    [JsonPropertyName("printType")]
    public ProductPrintType PrintType { get; set; }

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }

    /// <summary>Tổng tiền.</summary>
    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    /// <summary>Tổng tiền đã gồm VAT.</summary>
    [JsonPropertyName("totalPriceIncludeVAT")]
    public decimal TotalPriceIncludeVAT { get; set; }
}
