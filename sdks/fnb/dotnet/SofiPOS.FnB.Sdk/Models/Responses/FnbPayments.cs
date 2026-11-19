using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Kết quả tạm tính thanh toán (không ghi nhận thanh toán).</summary>
public sealed class PaymentPreview
{
    /// <summary>ID đơn hàng.</summary>
    [JsonPropertyName("orderId")]
    public Guid OrderId { get; set; }

    /// <summary>Tổng tiền món (đã gồm VAT).</summary>
    [JsonPropertyName("productAmount")]
    public decimal ProductAmount { get; set; }

    /// <summary>Tổng tiền combo (đã gồm VAT).</summary>
    [JsonPropertyName("comboAmount")]
    public decimal ComboAmount { get; set; }

    /// <summary>Tổng tiền món và combo (đã gồm VAT).</summary>
    [JsonPropertyName("subTotal")]
    public decimal SubTotal { get; set; }

    /// <summary>Tiền phụ thu (đã gồm VAT).</summary>
    [JsonPropertyName("surchargeAmount")]
    public decimal SurchargeAmount { get; set; }

    /// <summary>Tiền chiết khấu (đã gồm VAT).</summary>
    [JsonPropertyName("discountAmount")]
    public decimal DiscountAmount { get; set; }

    /// <summary>Tiền giảm voucher (đã gồm VAT).</summary>
    [JsonPropertyName("voucherAmount")]
    public decimal VoucherAmount { get; set; }

    /// <summary>Tiền trừ bằng điểm (đã gồm VAT).</summary>
    [JsonPropertyName("membershipAmount")]
    public decimal MembershipAmount { get; set; }

    /// <summary>Tổng phải thu (đã gồm VAT), làm tròn đồng.</summary>
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    /// <summary>Giá trị giảm.</summary>
    [JsonPropertyName("discount")]
    public OrderDiscount? Discount { get; set; }

    /// <summary>Giá trị phụ thu.</summary>
    [JsonPropertyName("surcharge")]
    public OrderSurcharge? Surcharge { get; set; }

    /// <summary>Voucher sau khi server tra mã và tính lại khoản giảm.</summary>
    [JsonPropertyName("voucher")]
    public OrderVoucher? Voucher { get; set; }
}

/// <summary>Mã QR thanh toán.</summary>
public sealed class PaymentQr
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Chuỗi dữ liệu mã QR.</summary>
    [JsonPropertyName("data")]
    public string? Data { get; set; }

    /// <summary>Ảnh mã QR dạng base64.</summary>
    [JsonPropertyName("base64Data")]
    public string? Base64Data { get; set; }

    /// <summary>Nhà cung cấp thanh toán.</summary>
    [JsonPropertyName("paymentProvider")]
    public string? PaymentProvider { get; set; }

    /// <summary>Mã ngân hàng.</summary>
    [JsonPropertyName("bankCode")]
    public string? BankCode { get; set; }

    /// <summary>Đường dẫn.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    /// <summary>Tên chủ tài khoản nhận tiền.</summary>
    [JsonPropertyName("accountName")]
    public string? AccountName { get; set; }

    /// <summary>Số tài khoản nhận tiền.</summary>
    [JsonPropertyName("accountNumber")]
    public string? AccountNumber { get; set; }

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}

/// <summary>Thông tin xuất hóa đơn điện tử.</summary>
public sealed class OrderInvoiceInfo
{
    /// <summary>Số CCCD người mua.</summary>
    [JsonPropertyName("buyerCCCD")]
    public string? BuyerCCCD { get; set; }

    /// <summary>Số hộ chiếu người mua.</summary>
    [JsonPropertyName("buyerHoChieu")]
    public string? BuyerHoChieu { get; set; }

    /// <summary>Tên người mua.</summary>
    [JsonPropertyName("buyerName")]
    public string? BuyerName { get; set; }

    /// <summary>Địa chỉ người mua.</summary>
    [JsonPropertyName("buyerAddress")]
    public string? BuyerAddress { get; set; }

    /// <summary>Số điện thoại người mua.</summary>
    [JsonPropertyName("buyerPhone")]
    public string? BuyerPhone { get; set; }

    /// <summary>Mã số thuế đơn vị mua.</summary>
    [JsonPropertyName("companyTaxCode")]
    public string? CompanyTaxCode { get; set; }

    /// <summary>Tên đơn vị mua.</summary>
    [JsonPropertyName("companyName")]
    public string? CompanyName { get; set; }

    /// <summary>Email nhận hóa đơn của đơn vị mua.</summary>
    [JsonPropertyName("companyEmail")]
    public string? CompanyEmail { get; set; }

    /// <summary>Địa chỉ đơn vị mua.</summary>
    [JsonPropertyName("companyAddress")]
    public string? CompanyAddress { get; set; }

    /// <summary>Số điện thoại đơn vị mua.</summary>
    [JsonPropertyName("companyPhone")]
    public string? CompanyPhone { get; set; }

    /// <summary>Xuất hóa đơn cho doanh nghiệp.</summary>
    [JsonPropertyName("isBusinessInvoice")]
    public bool IsBusinessInvoice { get; set; }

    /// <summary>Cho phép sửa thông tin hóa đơn.</summary>
    [JsonPropertyName("canEdit")]
    public bool CanEdit { get; set; }

    /// <summary>Đường dẫn tra cứu hóa đơn.</summary>
    [JsonPropertyName("lookupUrl")]
    public string? LookupUrl { get; set; }
}

/// <summary>Voucher áp dụng cho đơn.</summary>
public sealed class OrderVoucher
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Mã voucher.</summary>
    [JsonPropertyName("voucherCode")]
    public string? VoucherCode { get; set; }

    /// <summary>Giá trị giảm.</summary>
    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    /// <summary>Giá trị giảm đã gồm VAT.</summary>
    [JsonPropertyName("discountIncludeVAT")]
    public decimal DiscountIncludeVAT { get; set; }

    /// <summary>Kiểu giảm giá.</summary>
    [JsonPropertyName("discountType")]
    public DiscountType DiscountType { get; set; }

    /// <summary>Giá trị đơn tối thiểu; 0 là không giới hạn.</summary>
    [JsonPropertyName("min")]
    public decimal Min { get; set; }

    /// <summary>Số tiền giảm tối đa; 0 là không giới hạn.</summary>
    [JsonPropertyName("max")]
    public decimal Max { get; set; }

    /// <summary>Áp dụng trên giá trị trước VAT.</summary>
    [JsonPropertyName("isApplyBeforeVAT")]
    public bool IsApplyBeforeVAT { get; set; }

    /// <summary>Chi tiết theo từng mức thuế suất.</summary>
    [JsonPropertyName("details")]
    public List<OrderVoucherDetail>? Details { get; set; }

    /// <summary>Tiền thuế của khoản giảm.</summary>
    [JsonPropertyName("amountTax")]
    public decimal AmountTax { get; set; }

    /// <summary>Tổng tiền giảm trước VAT.</summary>
    [JsonPropertyName("amountDiscountBefortVAT")]
    public decimal AmountDiscountBefortVAT { get; set; }

    /// <summary>Tổng tiền giảm sau VAT.</summary>
    [JsonPropertyName("amountDiscountAfterVAT")]
    public decimal AmountDiscountAfterVAT { get; set; }
}

/// <summary>Thanh toán bằng điểm thành viên.</summary>
public sealed class OrderMembership
{
    /// <summary>ID hạng thành viên.</summary>
    [JsonPropertyName("rankId")]
    public Guid? RankId { get; set; }

    /// <summary>Tên hạng thành viên.</summary>
    [JsonPropertyName("rankName")]
    public string? RankName { get; set; }

    /// <summary>Trừ điểm trên giá trị trước hay sau VAT.</summary>
    [JsonPropertyName("pointBurnTaxBasis")]
    public PointBurnTaxBasis PointBurnTaxBasis { get; set; }

    /// <summary>Số điểm sử dụng.</summary>
    [JsonPropertyName("pointApplied")]
    public int PointApplied { get; set; }

    /// <summary>Tỷ lệ quy đổi điểm thành tiền.</summary>
    [JsonPropertyName("burnRate")]
    public decimal BurnRate { get; set; }

    /// <summary>Số tiền được trừ bằng điểm, trước VAT.</summary>
    [JsonPropertyName("amountAppliedBeforeVAT")]
    public decimal AmountAppliedBeforeVAT { get; set; }

    /// <summary>Số tiền được trừ bằng điểm, sau VAT.</summary>
    [JsonPropertyName("amountAppliedAfterVAT")]
    public decimal AmountAppliedAfterVAT { get; set; }

    /// <summary>Số tiền hoàn lại trước VAT.</summary>
    [JsonPropertyName("amountReturnBeforeVAT")]
    public decimal AmountReturnBeforeVAT { get; set; }

    /// <summary>Số tiền hoàn lại sau VAT.</summary>
    [JsonPropertyName("amountReturnAfterVAT")]
    public decimal AmountReturnAfterVAT { get; set; }
}

/// <summary>Chiết khấu của đơn.</summary>
public sealed class OrderDiscount
{
    /// <summary>Giá trị giảm.</summary>
    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    /// <summary>Cách áp dụng chiết khấu.</summary>
    [JsonPropertyName("discountApplyType")]
    public int DiscountApplyType { get; set; }

    /// <summary>Kiểu giảm giá.</summary>
    [JsonPropertyName("discountType")]
    public DiscountType DiscountType { get; set; }

    /// <summary>Áp dụng trên giá trị trước VAT.</summary>
    [JsonPropertyName("isApplyBeforeVAT")]
    public bool IsApplyBeforeVAT { get; set; }

    /// <summary>Thuế suất VAT áp cho khoản chiết khấu.</summary>
    [JsonPropertyName("vatDiscount")]
    public int VATDiscount { get; set; }

    /// <summary>ID người tạo.</summary>
    [JsonPropertyName("createById")]
    public Guid? CreateById { get; set; }

    /// <summary>Tên người tạo.</summary>
    [JsonPropertyName("createByName")]
    public string? CreateByName { get; set; }

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime? CreateDate { get; set; }

    /// <summary>Chi tiết theo từng mức thuế suất.</summary>
    [JsonPropertyName("details")]
    public List<OrderDiscountDetail> Details { get; set; } = new();

    /// <summary>Chương trình chiết khấu tự động sinh ra khoản này; null = chiết khấu tay.</summary>
    [JsonPropertyName("programId")]
    public Guid? ProgramId { get; set; }

    /// <summary>Version chương trình client đã dùng; server từ chối nếu khác Version hiện tại.</summary>
    [JsonPropertyName("programVersion")]
    public int? ProgramVersion { get; set; }

    /// <summary>Tên chương trình, chỉ server gán (bỏ qua giá trị client gửi).</summary>
    [JsonPropertyName("programName")]
    public string? ProgramName { get; set; }

    /// <summary>Tổng tiền giảm trước VAT.</summary>
    [JsonPropertyName("amountDiscountBefortVAT")]
    public decimal AmountDiscountBefortVAT { get; set; }

    /// <summary>Tổng tiền giảm sau VAT.</summary>
    [JsonPropertyName("amountDiscountAfterVAT")]
    public decimal AmountDiscountAfterVAT { get; set; }
}

/// <summary>Phụ thu của đơn.</summary>
public sealed class OrderSurcharge
{
    /// <summary>Giá trị phụ thu.</summary>
    [JsonPropertyName("surcharge")]
    public decimal Surcharge { get; set; }

    /// <summary>Kiểu phụ thu.</summary>
    [JsonPropertyName("surchargeType")]
    public DiscountType SurchargeType { get; set; }

    /// <summary>Áp dụng trên giá trị trước VAT.</summary>
    [JsonPropertyName("isApplyBeforeVAT")]
    public bool IsApplyBeforeVAT { get; set; }

    /// <summary>Lý do.</summary>
    [JsonPropertyName("reason")]
    public string Reason { get; set; } = string.Empty;

    /// <summary>ID người tạo.</summary>
    [JsonPropertyName("createById")]
    public Guid? CreateById { get; set; }

    /// <summary>Tên người tạo.</summary>
    [JsonPropertyName("createByName")]
    public string? CreateByName { get; set; }

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime? CreateDate { get; set; }

    /// <summary>Chi tiết theo từng mức thuế suất.</summary>
    [JsonPropertyName("details")]
    public List<OrderSurchargeDetail> Details { get; set; } = new();

    /// <summary>Chương trình phụ thu tự động sinh ra khoản này; null = phụ thu tay.</summary>
    [JsonPropertyName("programId")]
    public Guid? ProgramId { get; set; }

    /// <summary>Version chương trình client đã dùng; server từ chối nếu khác Version hiện tại.</summary>
    [JsonPropertyName("programVersion")]
    public int? ProgramVersion { get; set; }

    /// <summary>Tên chương trình, chỉ server gán (bỏ qua giá trị client gửi).</summary>
    [JsonPropertyName("programName")]
    public string? ProgramName { get; set; }

    /// <summary>Tổng tiền phụ thu trước VAT.</summary>
    [JsonPropertyName("amountSurchargeBeforeVAT")]
    public decimal AmountSurchargeBeforeVAT { get; set; }

    /// <summary>Tổng tiền phụ thu sau VAT.</summary>
    [JsonPropertyName("amountSurchargeAfterVAT")]
    public decimal AmountSurchargeAfterVAT { get; set; }
}

/// <summary>Tiền giảm voucher theo thuế suất.</summary>
public sealed class OrderVoucherDetail
{
    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Tiền giảm trước VAT.</summary>
    [JsonPropertyName("discountBefortVAT")]
    public decimal DiscountBefortVAT { get; set; }

    /// <summary>Tiền giảm sau VAT.</summary>
    [JsonPropertyName("discountAfterVAT")]
    public decimal DiscountAfterVAT { get; set; }

    /// <summary>Tiền giảm hoàn lại trước VAT.</summary>
    [JsonPropertyName("discountReturnBefortVAT")]
    public decimal DiscountReturnBefortVAT { get; set; }

    /// <summary>Tiền giảm hoàn lại sau VAT.</summary>
    [JsonPropertyName("discountReturnAfterVAT")]
    public decimal DiscountReturnAfterVAT { get; set; }
}

/// <summary>Tiền chiết khấu theo thuế suất.</summary>
public sealed class OrderDiscountDetail
{
    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Tiền giảm trước VAT.</summary>
    [JsonPropertyName("discountBefortVAT")]
    public decimal DiscountBefortVAT { get; set; }

    /// <summary>Tiền giảm sau VAT.</summary>
    [JsonPropertyName("discountAfterVAT")]
    public decimal DiscountAfterVAT { get; set; }

    /// <summary>Tiền giảm hoàn lại trước VAT.</summary>
    [JsonPropertyName("discountReturnBefortVAT")]
    public decimal DiscountReturnBefortVAT { get; set; }

    /// <summary>Tiền giảm hoàn lại sau VAT.</summary>
    [JsonPropertyName("discountReturnAfterVAT")]
    public decimal DiscountReturnAfterVAT { get; set; }
}

/// <summary>Tiền phụ thu theo thuế suất.</summary>
public sealed class OrderSurchargeDetail
{
    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Tiền phụ thu trước VAT.</summary>
    [JsonPropertyName("surchargeBeforeVAT")]
    public decimal SurchargeBeforeVAT { get; set; }

    /// <summary>Tiền phụ thu hoàn lại trước VAT.</summary>
    [JsonPropertyName("surchargeReturnBeforeVAT")]
    public decimal SurchargeReturnBeforeVAT { get; set; }

    /// <summary>Tiền phụ thu hoàn lại sau VAT.</summary>
    [JsonPropertyName("surchargeReturnAfterVAT")]
    public decimal SurchargeReturnAfterVAT { get; set; }

    /// <summary>Tiền phụ thu sau VAT.</summary>
    [JsonPropertyName("surchargeAfterVAT")]
    public decimal SurchargeAfterVAT { get; set; }
}
