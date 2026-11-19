using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Tạo sản phẩm.</summary>
public sealed class ProductCreateRequest
{
    /// <summary>Mã sản phẩm trên POS (bắt buộc, tối đa 300 ký tự, phân biệt hoa thường).</summary>
    [JsonPropertyName("productCode")]
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>Tên sản phẩm (bắt buộc, tối đa 500 ký tự).</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Giá bán đã gồm VAT, mặc định 0.</summary>
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    /// <summary>Đơn vị tính (tối đa 30 ký tự), mặc định <c>Cái</c>.</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}

/// <summary>Cập nhật sản phẩm.</summary>
public sealed class ProductUpdateRequest
{
    /// <summary>Tên sản phẩm (bắt buộc, tối đa 500 ký tự).</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Giá bán đã gồm VAT; <c>null</c> giữ giá cũ.</summary>
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    /// <summary>Đơn vị tính; <c>null</c> giữ giá trị cũ.</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}

/// <summary>Tạo hoặc sửa mã QR đặt món (chỉ chi nhánh FnB).</summary>
public sealed class QrOrderCodeRequest
{
    /// <summary>Chi nhánh FnB đang hoạt động (bắt buộc).</summary>
    [JsonPropertyName("branchId")]
    public Guid BranchId { get; set; }

    /// <summary>Tên QR, tối đa 200 ký tự.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Kênh bán (bắt buộc), lấy từ <c>QrOrderCodes.GetSaleChannelsAsync</c>.</summary>
    [JsonPropertyName("saleChannelId")]
    public string SaleChannelId { get; set; } = string.Empty;

    /// <summary>Bàn; bắt buộc khi kênh <c>TaiCho</c>, kênh khác bị bỏ qua.</summary>
    [JsonPropertyName("tableId")]
    public Guid? TableId { get; set; }

    /// <summary>Số giờ hiệu lực tính từ lúc lưu.</summary>
    [JsonPropertyName("hours")]
    public int Hours { get; set; }

    /// <summary>Số phút hiệu lực (0–59).</summary>
    [JsonPropertyName("minutes")]
    public int Minutes { get; set; }

    /// <summary><c>true</c>: không giới hạn thời gian, bỏ qua <see cref="Hours"/>/<see cref="Minutes"/>.</summary>
    [JsonPropertyName("noExpiry")]
    public bool NoExpiry { get; set; }

    /// <summary>Bắt buộc khách nhập tên và số điện thoại.</summary>
    [JsonPropertyName("requireCustomerInfo")]
    public bool RequireCustomerInfo { get; set; }

    /// <summary>QR chỉ tạo được 1 đơn.</summary>
    [JsonPropertyName("isSingleUse")]
    public bool IsSingleUse { get; set; }

    /// <summary>Thuộc tính đơn khách cần nhập, không trùng <c>attributeId</c>.</summary>
    [JsonPropertyName("attributes")]
    public List<QrOrderCodeAttributeRequest> Attributes { get; set; } = new();
}

/// <summary>Thuộc tính đơn khách cần nhập khi đặt món qua QR.</summary>
public sealed class QrOrderCodeAttributeRequest
{
    /// <summary>ID thuộc tính đơn hàng (từ <c>QrOrderCodes.GetOrderAttributesAsync</c>).</summary>
    [JsonPropertyName("attributeId")]
    public string AttributeId { get; set; } = string.Empty;

    /// <summary>Bắt buộc nhập.</summary>
    [JsonPropertyName("isRequired")]
    public bool IsRequired { get; set; }

    /// <summary>Thứ tự hiển thị.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }
}

/// <summary>Tạo hoặc sửa khách hàng (không gồm điểm và hạng thành viên).</summary>
public sealed class CustomerRequest
{
    /// <summary>Tên khách hàng (bắt buộc).</summary>
    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>Số điện thoại (bắt buộc, tối đa 20 ký tự, không trùng khách khác của cửa hàng).</summary>
    [JsonPropertyName("customerPhone")]
    public string CustomerPhone { get; set; } = string.Empty;

    /// <summary>Địa chỉ.</summary>
    [JsonPropertyName("customerAddress")]
    public string? CustomerAddress { get; set; }

    /// <summary>Số CCCD.</summary>
    [JsonPropertyName("customerCccd")]
    public string? CustomerCccd { get; set; }

    /// <summary>Giới tính: <c>true</c> nam, <c>false</c> nữ, <c>null</c> chưa xác định.</summary>
    [JsonPropertyName("gender")]
    public bool? Gender { get; set; }

    /// <summary>Ngày sinh.</summary>
    [JsonPropertyName("birthDate")]
    public DateTime? BirthDate { get; set; }
}

/// <summary>Tạo hoặc sửa chính sách giá sản phẩm.</summary>
public sealed class PricingPolicyRequest
{
    /// <summary>Tên chính sách (bắt buộc).</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Số nhỏ hơn được ưu tiên khi nhiều chính sách cùng khớp.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Đang hoạt động. Mặc định <c>true</c>.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>Từ ngày (tính theo ngày).</summary>
    [JsonPropertyName("fromDate")]
    public DateTime? FromDate { get; set; }

    /// <summary>Đến ngày (tính theo ngày), không nhỏ hơn <see cref="FromDate"/>.</summary>
    [JsonPropertyName("toDate")]
    public DateTime? ToDate { get; set; }

    /// <summary>Chi nhánh áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("branchIds")]
    public List<Guid> BranchIds { get; set; } = new();

    /// <summary>Kênh bán áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("saleChannelIds")]
    public List<string> SaleChannelIds { get; set; } = new();

    /// <summary>Ngày trong tuần áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("daysOfWeek")]
    public List<DayOfWeek> DaysOfWeek { get; set; } = new();

    /// <summary>Giá sản phẩm; sản phẩm và đơn vị tính không trùng nhau.</summary>
    [JsonPropertyName("products")]
    public List<PricingPolicyProduct> Products { get; set; } = new();

    /// <summary>Khung giờ áp dụng, không chồng lấn; rỗng là cả ngày.</summary>
    [JsonPropertyName("timeSlots")]
    public List<TimeSlot> TimeSlots { get; set; } = new();
}

/// <summary>Thêm kênh bán cho cửa hàng.</summary>
public sealed class SaleChannelCreateRequest
{
    /// <summary>ID kênh trong danh mục hệ thống (bắt buộc), ví dụ <c>GiaoHang</c>.</summary>
    [JsonPropertyName("saleChannelId")]
    public string SaleChannelId { get; set; } = string.Empty;

    /// <summary>Đang hoạt động. Mặc định <c>true</c>.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Loại kênh bán.</summary>
    [JsonPropertyName("isECommerce")]
    public SaleChannelECommerceType IsECommerce { get; set; } = SaleChannelECommerceType.Offline;
}

/// <summary>Sửa kênh bán của cửa hàng.</summary>
public sealed class SaleChannelUpdateRequest
{
    /// <summary>ID kênh, SDK tự điền theo tham số <c>saleChannelId</c> của phương thức.</summary>
    [JsonPropertyName("saleChannelId")]
    public string SaleChannelId { get; set; } = string.Empty;

    /// <summary>Đang hoạt động. Kênh <c>TaiCho</c> không được tắt.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Loại kênh bán.</summary>
    [JsonPropertyName("isECommerce")]
    public SaleChannelECommerceType IsECommerce { get; set; } = SaleChannelECommerceType.Offline;
}

/// <summary>Tạo hoặc sửa combo. Combo đã phát sinh đơn chỉ cập nhật được <see cref="IsActive"/>.</summary>
public sealed class ComboRequest
{
    /// <summary>Tên combo (bắt buộc).</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Mô tả.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Loại combo: cố định dùng <see cref="FixedProducts"/>, tự chọn dùng <see cref="Groups"/>.</summary>
    [JsonPropertyName("comboType")]
    public ComboType ComboType { get; set; }

    /// <summary>Cách tính giá.</summary>
    [JsonPropertyName("pricingMode")]
    public ComboPricingMode PricingMode { get; set; }

    /// <summary>Giá chung của combo, chưa gồm VAT (khi <see cref="PricingMode"/> là <see cref="ComboPricingMode.Common"/>).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>Thuế suất VAT (%) của giá chung.</summary>
    [JsonPropertyName("vat")]
    public int Vat { get; set; }

    /// <summary>Mã ngành nghề tính thuế.</summary>
    [JsonPropertyName("taxIndustryCategoryId")]
    public string? TaxIndustryCategoryId { get; set; }

    /// <summary>Số lần sử dụng (combo dịch vụ nhiều lần). Mặc định 1.</summary>
    [JsonPropertyName("usageLimit")]
    public int? UsageLimit { get; set; } = 1;

    /// <summary>Thời hạn sử dụng (ngày) tính từ ngày mua (combo dịch vụ).</summary>
    [JsonPropertyName("expiredInDays")]
    public int? ExpiredInDays { get; set; }

    /// <summary>Ảnh; <c>null</c> giữ ảnh hiện tại.</summary>
    [JsonPropertyName("image")]
    public ImageUpload? Image { get; set; }

    /// <summary>Sản phẩm của combo cố định.</summary>
    [JsonPropertyName("fixedProducts")]
    public List<ComboProductRequest> FixedProducts { get; set; } = new();

    /// <summary>Nhóm lựa chọn của combo tự chọn.</summary>
    [JsonPropertyName("groups")]
    public List<ComboGroupRequest> Groups { get; set; } = new();
}

/// <summary>Nhóm lựa chọn của combo tự chọn.</summary>
public sealed class ComboGroupRequest
{
    /// <summary>Tên nhóm.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Số món khách cần chọn trong nhóm.</summary>
    [JsonPropertyName("requiredQuantity")]
    public int RequiredQuantity { get; set; }

    /// <summary>Sản phẩm được chọn trong nhóm.</summary>
    [JsonPropertyName("products")]
    public List<ComboProductRequest> Products { get; set; } = new();
}

/// <summary>Sản phẩm trong combo. Sản phẩm trùng nhau phải khác đơn vị tính.</summary>
public sealed class ComboProductRequest
{
    /// <summary>ID sản phẩm.</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>ID đơn vị tính; <c>null</c> là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }

    /// <summary>Tên đơn vị tính.</summary>
    [JsonPropertyName("unitName")]
    public string? UnitName { get; set; }

    /// <summary>Số lượng. Mặc định 1.</summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 1;

    /// <summary>Thành tiền của sản phẩm trong combo (chưa gồm VAT).</summary>
    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }
}

/// <summary>Tạo hoặc sửa flash sale. Flash sale đã phát sinh đơn chỉ cập nhật được <see cref="IsActive"/>.</summary>
public sealed class FlashSaleRequest
{
    /// <summary>Tên flash sale (bắt buộc).</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Mô tả.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Thời điểm bắt đầu (bắt buộc).</summary>
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>Thời điểm kết thúc; <c>null</c> là không giới hạn.</summary>
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>Ảnh; <c>null</c> giữ ảnh hiện tại.</summary>
    [JsonPropertyName("image")]
    public ImageUpload? Image { get; set; }

    /// <summary>Chi nhánh áp dụng.</summary>
    [JsonPropertyName("branchIds")]
    public List<Guid> BranchIds { get; set; } = new();

    /// <summary>Kênh bán áp dụng (đã cấu hình cho cửa hàng).</summary>
    [JsonPropertyName("saleChannelIds")]
    public List<string> SaleChannelIds { get; set; } = new();

    /// <summary>Sản phẩm và giá flash sale.</summary>
    [JsonPropertyName("products")]
    public List<FlashSaleProductRequest> Products { get; set; } = new();
}

/// <summary>Sản phẩm trong flash sale. Sản phẩm trùng nhau phải khác đơn vị tính.</summary>
public sealed class FlashSaleProductRequest
{
    /// <summary>ID sản phẩm.</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>ID đơn vị tính; <c>null</c> là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }

    /// <summary>Tên đơn vị tính.</summary>
    [JsonPropertyName("unitName")]
    public string? UnitName { get; set; }

    /// <summary>Giá flash sale.</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}

/// <summary>
/// Tạo hoặc sửa voucher. Voucher tạo mới luôn chưa hoạt động (bật bằng <c>Vouchers.SetActiveAsync</c>);
/// khi sửa, <see cref="IsActive"/> được ghi theo giá trị gửi lên.
/// </summary>
public sealed class VoucherRequest
{
    /// <summary>Tên voucher (bắt buộc).</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Mô tả ngắn.</summary>
    [JsonPropertyName("brief")]
    public string? Brief { get; set; }

    /// <summary>Nội dung chi tiết.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>Ngày bắt đầu.</summary>
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>Ngày kết thúc, không nhỏ hơn ngày bắt đầu; khi tạo không nhỏ hơn hiện tại.</summary>
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>Trạng thái hoạt động (bị bỏ qua khi tạo).</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Kiểu giảm: theo % hoặc số tiền.</summary>
    [JsonPropertyName("discountType")]
    public DiscountType DiscountType { get; set; } = DiscountType.Percent;

    /// <summary>Giá trị giảm.</summary>
    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    /// <summary>Số tiền giảm tối đa; 0 là không giới hạn.</summary>
    [JsonPropertyName("max")]
    public decimal Max { get; set; }

    /// <summary>Giá trị đơn tối thiểu; 0 là không giới hạn.</summary>
    [JsonPropertyName("min")]
    public decimal Min { get; set; }

    /// <summary>Số lần dùng mỗi mã; 0 là không giới hạn.</summary>
    [JsonPropertyName("usageLimit")]
    public int UsageLimit { get; set; }

    /// <summary>Số lượng mã (lớn hơn 0); hệ thống sinh mã 6 ký tự viết hoa.</summary>
    [JsonPropertyName("voucherNumber")]
    public int VoucherNumber { get; set; } = 1;

    /// <summary>Mã nhập tay, chỉ dùng khi <see cref="VoucherNumber"/> bằng 1; không trùng mã hiện có.</summary>
    [JsonPropertyName("voucherCode")]
    public string? VoucherCode { get; set; }

    /// <summary>Áp dụng trên giá trị trước VAT. Mặc định <c>true</c>.</summary>
    [JsonPropertyName("isApplyBeforeVAT")]
    public bool IsApplyBeforeVat { get; set; } = true;

    /// <summary>Ảnh; <c>null</c> giữ ảnh hiện tại.</summary>
    [JsonPropertyName("image")]
    public ImageUpload? Image { get; set; }
}

/// <summary>Tạo hoặc sửa chương trình chiết khấu/phụ thu theo đơn.</summary>
public sealed class OrderAdjustmentProgramRequest
{
    /// <summary>Tên chương trình (bắt buộc, tối đa 200 ký tự).</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Loại: chiết khấu hoặc phụ thu.</summary>
    [JsonPropertyName("type")]
    public OrderAdjustmentType Type { get; set; } = OrderAdjustmentType.Discount;

    /// <summary>Kiểu giá trị: theo % hoặc số tiền.</summary>
    [JsonPropertyName("valueType")]
    public DiscountType ValueType { get; set; } = DiscountType.Percent;

    /// <summary>Giá trị.</summary>
    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    /// <summary>Áp dụng trên giá trị trước VAT.</summary>
    [JsonPropertyName("isApplyBeforeVAT")]
    public bool IsApplyBeforeVat { get; set; }

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Đang hoạt động. Mặc định <c>true</c>.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; } = true;

    /// <summary>Từ ngày.</summary>
    [JsonPropertyName("fromDate")]
    public DateTime? FromDate { get; set; }

    /// <summary>Đến ngày.</summary>
    [JsonPropertyName("toDate")]
    public DateTime? ToDate { get; set; }

    /// <summary>
    /// Phiên bản đã đọc từ chi tiết, gửi lại khi sửa để chống ghi đè (sai trả 422). <c>null</c> thì không kiểm tra.
    /// </summary>
    [JsonPropertyName("version")]
    public int? Version { get; set; }

    /// <summary>Chi nhánh áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("branchIds")]
    public List<Guid> BranchIds { get; set; } = new();

    /// <summary>Kênh bán áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("saleChannelIds")]
    public List<string> SaleChannelIds { get; set; } = new();

    /// <summary>Ngày trong tuần áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("daysOfWeek")]
    public List<DayOfWeek> DaysOfWeek { get; set; } = new();

    /// <summary>Khung giờ áp dụng; rỗng là cả ngày.</summary>
    [JsonPropertyName("timeSlots")]
    public List<TimeSlot> TimeSlots { get; set; } = new();

    /// <summary>Hạng thành viên áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("membershipRankIds")]
    public List<Guid> MembershipRankIds { get; set; } = new();
}
