using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Giá trị chọn cho một thuộc tính (thuộc tính món hoặc thuộc tính đơn).</summary>
public sealed class AttributeValue
{
    /// <summary>Tạo giá trị thuộc tính.</summary>
    public AttributeValue()
    {
    }

    /// <summary>Tạo giá trị thuộc tính.</summary>
    /// <param name="id">ID thuộc tính.</param>
    /// <param name="value">Giá trị chọn.</param>
    public AttributeValue(string id, string? value)
    {
        Id = id;
        Value = value;
    }

    /// <summary>ID thuộc tính.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>Giá trị chọn.</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}

/// <summary>Tạo đơn FnB. Giá do server xác định theo kênh bán, chính sách giá, flash sale.</summary>
public sealed class FnbOrderCreateRequest
{
    /// <summary>
    /// Khóa chống tạo trùng do đối tác sinh. Gọi lại với cùng giá trị trả về đơn đã tạo thay vì tạo đơn mới.
    /// Nên luôn đặt giá trị này (ví dụ <see cref="Guid.NewGuid"/>) và lưu lại trước khi gọi API.
    /// </summary>
    [JsonPropertyName("requestId")]
    public Guid? RequestId { get; set; }

    /// <summary>Kênh bán đang hoạt động. Mặc định <c>TaiCho</c>.</summary>
    [JsonPropertyName("saleChannelId")]
    public string SaleChannelId { get; set; } = "TaiCho";

    /// <summary>Bàn thuộc chi nhánh, đang hoạt động và chưa có đơn.</summary>
    [JsonPropertyName("tableId")]
    public Guid? TableId { get; set; }

    /// <summary>Ghi chú (tối đa 1000 ký tự).</summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>Khách hàng của cửa hàng.</summary>
    [JsonPropertyName("customerId")]
    public Guid? CustomerId { get; set; }

    /// <summary>Món gọi.</summary>
    [JsonPropertyName("items")]
    public List<FnbOrderItemRequest> Items { get; set; } = new();

    /// <summary>Combo gọi.</summary>
    [JsonPropertyName("combos")]
    public List<FnbOrderComboRequest> Combos { get; set; } = new();

    /// <summary>Thuộc tính đơn hàng.</summary>
    [JsonPropertyName("attributes")]
    public List<AttributeValue> Attributes { get; set; } = new();
}

/// <summary>Một món gọi trong đơn.</summary>
public sealed class FnbOrderItemRequest
{
    /// <summary>ID sản phẩm (món đang bán).</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>Số lượng. Mặc định 1.</summary>
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; } = 1;

    /// <summary>Ghi chú món.</summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>ID đơn vị tính; <c>null</c> là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }

    /// <summary>ID sản phẩm flash sale lấy từ menu (<c>FnbMenuProduct.FlashSaleProductId</c>).</summary>
    [JsonPropertyName("flashSaleProductId")]
    public Guid? FlashSaleProductId { get; set; }

    /// <summary>Thuộc tính món (thuộc nhóm sản phẩm của món).</summary>
    [JsonPropertyName("attributes")]
    public List<AttributeValue> Attributes { get; set; } = new();

    /// <summary>Món kèm (thuộc cấu hình món kèm của món chính).</summary>
    [JsonPropertyName("addOns")]
    public List<FnbOrderAddOnRequest> AddOns { get; set; } = new();
}

/// <summary>Món kèm của một món.</summary>
public sealed class FnbOrderAddOnRequest
{
    /// <summary>ID sản phẩm món kèm.</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>Số lượng. Mặc định 1.</summary>
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; } = 1;

    /// <summary>ID đơn vị tính; <c>null</c> là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }
}

/// <summary>Một combo gọi trong đơn. Số lượng N tạo N dòng combo.</summary>
public sealed class FnbOrderComboRequest
{
    /// <summary>ID combo đang bán.</summary>
    [JsonPropertyName("comboId")]
    public Guid ComboId { get; set; }

    /// <summary>Số lượng. Mặc định 1.</summary>
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; } = 1;

    /// <summary>Ghi chú.</summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>Lựa chọn cho từng nhóm của combo tự chọn.</summary>
    [JsonPropertyName("groups")]
    public List<FnbOrderComboGroupRequest> Groups { get; set; } = new();
}

/// <summary>Lựa chọn trong một nhóm của combo tự chọn.</summary>
public sealed class FnbOrderComboGroupRequest
{
    /// <summary>ID nhóm của combo.</summary>
    [JsonPropertyName("groupId")]
    public Guid GroupId { get; set; }

    /// <summary>Món được chọn.</summary>
    [JsonPropertyName("products")]
    public List<FnbOrderComboProductRequest> Products { get; set; } = new();
}

/// <summary>Món được chọn trong nhóm combo.</summary>
public sealed class FnbOrderComboProductRequest
{
    /// <summary>ID sản phẩm.</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>Số lượng. Mặc định 1.</summary>
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; } = 1;

    /// <summary>ID đơn vị tính; <c>null</c> là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }
}

/// <summary>Chuyển một phần món/combo sang đơn khác hoặc tách sang đơn mới.</summary>
public sealed class FnbMoveItemsRequest
{
    /// <summary>Đơn đích đang mở cùng chi nhánh; <c>null</c> để tạo đơn mới.</summary>
    [JsonPropertyName("targetOrderId")]
    public Guid? TargetOrderId { get; set; }

    /// <summary>Kênh bán của đơn mới. Mặc định <c>TaiCho</c>.</summary>
    [JsonPropertyName("saleChannelId")]
    public string SaleChannelId { get; set; } = "TaiCho";

    /// <summary>Bàn của đơn mới (khi không có <see cref="TargetOrderId"/>), phải còn trống.</summary>
    [JsonPropertyName("targetTableId")]
    public Guid? TargetTableId { get; set; }

    /// <summary>Dòng món cần chuyển.</summary>
    [JsonPropertyName("products")]
    public List<FnbMoveItem> Products { get; set; } = new();

    /// <summary>Dòng combo cần chuyển.</summary>
    [JsonPropertyName("combos")]
    public List<FnbMoveItem> Combos { get; set; } = new();
}

/// <summary>Dòng món/combo cần chuyển.</summary>
public sealed class FnbMoveItem
{
    /// <summary>lineId của dòng trong đơn nguồn.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Số lượng chuyển.</summary>
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    /// <summary>Dòng món kèm đi cùng món chính.</summary>
    [JsonPropertyName("childs")]
    public List<FnbMoveChildItem> Childs { get; set; } = new();
}

/// <summary>Dòng món kèm đi cùng món chính khi chuyển.</summary>
public sealed class FnbMoveChildItem
{
    /// <summary>lineId của dòng món kèm.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }
}

/// <summary>
/// Chiết khấu gửi khi tạm tính / tạo QR / thanh toán. Để <c>null</c> (không gửi) thì server tự áp chương trình chiết khấu
/// đang khớp đơn; dùng <see cref="None"/> để không áp chiết khấu.
/// </summary>
public sealed class OrderDiscountRequest
{
    /// <summary>Không áp chiết khấu (gửi <c>{"discount": 0}</c>).</summary>
    public static OrderDiscountRequest None => new() { Discount = 0 };

    /// <summary>Giá trị chiết khấu (theo % tối đa 100, hoặc số tiền).</summary>
    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    /// <summary>Kiểu chiết khấu.</summary>
    [JsonPropertyName("discountType")]
    public DiscountType DiscountType { get; set; } = DiscountType.Percent;

    /// <summary>Tính trên giá trị trước VAT.</summary>
    [JsonPropertyName("isApplyBeforeVAT")]
    public bool IsApplyBeforeVat { get; set; }
}

/// <summary>
/// Phụ thu gửi khi tạm tính / tạo QR / thanh toán. Để <c>null</c> thì server tự áp chương trình phụ thu đang khớp đơn;
/// dùng <see cref="None"/> để không áp phụ thu.
/// </summary>
public sealed class OrderSurchargeRequest
{
    /// <summary>Không áp phụ thu (gửi <c>{"surcharge": 0}</c>).</summary>
    public static OrderSurchargeRequest None => new() { Surcharge = 0 };

    /// <summary>Giá trị phụ thu.</summary>
    [JsonPropertyName("surcharge")]
    public decimal Surcharge { get; set; }

    /// <summary>Kiểu phụ thu.</summary>
    [JsonPropertyName("surchargeType")]
    public DiscountType SurchargeType { get; set; } = DiscountType.Percent;

    /// <summary>Tính trên giá trị trước VAT.</summary>
    [JsonPropertyName("isApplyBeforeVAT")]
    public bool IsApplyBeforeVat { get; set; }

    /// <summary>Lý do phụ thu.</summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }
}

/// <summary>Voucher áp dụng: chỉ cần mã, server tự kiểm tra và tính khoản giảm.</summary>
public sealed class OrderVoucherRequest
{
    /// <summary>Tạo voucher theo mã.</summary>
    public OrderVoucherRequest()
    {
    }

    /// <summary>Tạo voucher theo mã.</summary>
    /// <param name="voucherCode">Mã voucher.</param>
    public OrderVoucherRequest(string voucherCode)
    {
        VoucherCode = voucherCode;
    }

    /// <summary>Mã voucher.</summary>
    [JsonPropertyName("voucherCode")]
    public string VoucherCode { get; set; } = string.Empty;
}

/// <summary>Thanh toán một phần bằng điểm thành viên của khách trên đơn.</summary>
public sealed class OrderMembershipRequest
{
    /// <summary>Hạng thành viên của khách.</summary>
    [JsonPropertyName("rankId")]
    public Guid? RankId { get; set; }

    /// <summary>Số điểm sử dụng.</summary>
    [JsonPropertyName("pointApplied")]
    public int PointApplied { get; set; }

    /// <summary>Tỷ lệ quy đổi điểm thành tiền của hạng (<c>MembershipRank.BurnRate</c>).</summary>
    [JsonPropertyName("burnRate")]
    public decimal BurnRate { get; set; }

    /// <summary>Trừ trên giá trị trước hay sau VAT (<c>MembershipRank.PointBurnTaxBasis</c>).</summary>
    [JsonPropertyName("pointBurnTaxBasis")]
    public PointBurnTaxBasis PointBurnTaxBasis { get; set; } = PointBurnTaxBasis.AfterVat;
}

/// <summary>Dữ liệu tạm tính hoặc thanh toán đơn FnB.</summary>
public sealed class FnbPaymentRequest
{
    /// <summary>Chiết khấu; <c>null</c> để server tự áp chương trình đang khớp.</summary>
    [JsonPropertyName("discount")]
    public OrderDiscountRequest? Discount { get; set; }

    /// <summary>Phụ thu; <c>null</c> để server tự áp chương trình đang khớp.</summary>
    [JsonPropertyName("surcharge")]
    public OrderSurchargeRequest? Surcharge { get; set; }

    /// <summary>Voucher.</summary>
    [JsonPropertyName("voucher")]
    public OrderVoucherRequest? Voucher { get; set; }

    /// <summary>Điểm thành viên.</summary>
    [JsonPropertyName("membership")]
    public OrderMembershipRequest? Membership { get; set; }

    /// <summary>Thông tin xuất hóa đơn điện tử.</summary>
    [JsonPropertyName("invoiceInfo")]
    public OrderInvoiceInfo? InvoiceInfo { get; set; }

    /// <summary>Phương thức thanh toán. Mặc định tiền mặt.</summary>
    [JsonPropertyName("paymentMethod")]
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

    /// <summary>Nhà cung cấp thanh toán (<c>PaymentOption.Provider</c>) khi thanh toán qua ngân hàng/QR.</summary>
    [JsonPropertyName("paymentProvider")]
    public string? PaymentProvider { get; set; }

    /// <summary>Số tiền khách trả.</summary>
    [JsonPropertyName("customerPayment")]
    public decimal CustomerPayment { get; set; }
}

/// <summary>Tạo QR thanh toán cho đơn.</summary>
public sealed class FnbPaymentQrRequest
{
    /// <summary>Nhà cung cấp QR (<c>PaymentOption.Provider</c> có <c>SupportsQr</c>), bắt buộc.</summary>
    [JsonPropertyName("paymentProvider")]
    public string PaymentProvider { get; set; } = string.Empty;

    /// <summary>Số tiền của QR, không lớn hơn tổng tiền đơn.</summary>
    [JsonPropertyName("customerPayment")]
    public decimal CustomerPayment { get; set; }

    /// <summary>Kích thước điểm ảnh của ảnh QR. Mặc định 1.</summary>
    [JsonPropertyName("pixel")]
    public int Pixel { get; set; } = 1;

    /// <summary>Chiết khấu; <c>null</c> để server tự áp chương trình đang khớp.</summary>
    [JsonPropertyName("discount")]
    public OrderDiscountRequest? Discount { get; set; }

    /// <summary>Phụ thu; <c>null</c> để server tự áp chương trình đang khớp.</summary>
    [JsonPropertyName("surcharge")]
    public OrderSurchargeRequest? Surcharge { get; set; }

    /// <summary>Voucher.</summary>
    [JsonPropertyName("voucher")]
    public OrderVoucherRequest? Voucher { get; set; }

    /// <summary>Điểm thành viên.</summary>
    [JsonPropertyName("membership")]
    public OrderMembershipRequest? Membership { get; set; }
}
