using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Menu bán hàng FnB theo chi nhánh và kênh bán.</summary>
public sealed class FnbMenu
{
    /// <summary>ID chi nhánh.</summary>
    [JsonPropertyName("branchId")]
    public Guid BranchId { get; set; }

    /// <summary>ID kênh bán (ví dụ <c>TaiCho</c>).</summary>
    [JsonPropertyName("saleChannelId")]
    public string SaleChannelId { get; set; } = string.Empty;

    /// <summary>Thời điểm tính giá.</summary>
    [JsonPropertyName("pricedAt")]
    public DateTime PricedAt { get; set; }

    /// <summary>Chi nhánh bật Menu FnB: chỉ bán món trong nhóm menu đang hoạt động, theo giới hạn trong ngày.</summary>
    [JsonPropertyName("isMenuEnabled")]
    public bool IsMenuEnabled { get; set; }

    /// <summary>Nhóm Menu FnB (khi chi nhánh bật Menu).</summary>
    [JsonPropertyName("menuGroups")]
    public List<FnbMenuGroup> MenuGroups { get; set; } = new();

    /// <summary>Nhóm món.</summary>
    [JsonPropertyName("categories")]
    public List<FnbMenuCategory> Categories { get; set; } = new();

    /// <summary>Danh sách sản phẩm.</summary>
    [JsonPropertyName("products")]
    public List<FnbMenuProduct> Products { get; set; } = new();

    /// <summary>Danh sách combo.</summary>
    [JsonPropertyName("combos")]
    public List<FnbMenuCombo> Combos { get; set; } = new();
}

/// <summary>Món đang bán (giá theo kênh bán, chính sách giá, flash sale).</summary>
public sealed class FnbMenuProduct
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Mã.</summary>
    [JsonPropertyName("code")]
    public string Code { get; set; } = string.Empty;

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>ID nhóm sản phẩm.</summary>
    [JsonPropertyName("categoryId")]
    public Guid CategoryId { get; set; }

    /// <summary>Tên nhóm sản phẩm.</summary>
    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>Giá đã gồm VAT.</summary>
    [JsonPropertyName("priceIncludeVAT")]
    public decimal PriceIncludeVAT { get; set; }

    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Tên đơn vị tính.</summary>
    [JsonPropertyName("unitName")]
    public string UnitName { get; set; } = string.Empty;

    /// <summary>ID đơn vị tính; <c>null</c> hoặc GUID rỗng là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>Có quản lý tồn kho.</summary>
    [JsonPropertyName("isInventory")]
    public bool IsInventory { get; set; }

    /// <summary>Kiểu in của món.</summary>
    [JsonPropertyName("printType")]
    public ProductPrintType PrintType { get; set; }

    /// <summary>Loại.</summary>
    [JsonPropertyName("type")]
    public ProductType Type { get; set; }

    /// <summary>Thuộc diện giảm thuế GTGT.</summary>
    [JsonPropertyName("isTaxReductionApplicable")]
    public bool IsTaxReductionApplicable { get; set; }

    /// <summary>Giá flash sale (chưa gồm VAT); <c>null</c> nếu không có.</summary>
    [JsonPropertyName("flashSalePrice")]
    public decimal? FlashSalePrice { get; set; }

    /// <summary>Giá flash sale đã gồm VAT.</summary>
    [JsonPropertyName("flashSalePriceIncludeVAT")]
    public decimal? FlashSalePriceIncludeVAT { get; set; }

    /// <summary>Tên flash sale.</summary>
    [JsonPropertyName("flashSaleTitle")]
    public string? FlashSaleTitle { get; set; }

    /// <summary>ID sản phẩm trong flash sale, gửi kèm khi gọi món theo giá flash sale.</summary>
    [JsonPropertyName("flashSaleProductId")]
    public Guid? FlashSaleProductId { get; set; }
}

/// <summary>Thuộc tính (của món hoặc của đơn hàng) và giá trị đã chọn.</summary>
public sealed class ProductAttribute
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Loại.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>Giá trị.</summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Các giá trị có thể chọn.</summary>
    [JsonPropertyName("values")]
    public List<string> Values { get; set; } = new();
}

/// <summary>Phương thức thanh toán của chi nhánh.</summary>
public sealed class PaymentOption
{
    /// <summary>Phương thức ghi nhận khi thanh toán: 1 tiền mặt, 2 ngân hàng.</summary>
    [JsonPropertyName("paymentMethod")]
    public int PaymentMethod { get; set; }

    /// <summary>Nhà cung cấp QR (paymentProvider) hoặc null với tiền mặt.</summary>
    [JsonPropertyName("provider")]
    public string? Provider { get; set; }

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Đang bật.</summary>
    [JsonPropertyName("isEnabled")]
    public bool IsEnabled { get; set; }

    /// <summary>Có tạo QR qua POST /orders/{orderId}/qr.</summary>
    [JsonPropertyName("supportsQr")]
    public bool SupportsQr { get; set; }

    /// <summary>Có tra cứu giao dịch qua GET /orders/{orderId}/qr/check/{provider}.</summary>
    [JsonPropertyName("supportsTransactionCheck")]
    public bool SupportsTransactionCheck { get; set; }
}

/// <summary>Khu vực và các bàn.</summary>
public sealed class FnbArea
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Danh sách bàn.</summary>
    [JsonPropertyName("tables")]
    public List<FnbTable>? Tables { get; set; }
}

/// <summary>Nhóm Menu FnB.</summary>
public sealed class FnbMenuGroup
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Thứ tự hiển thị.</summary>
    [JsonPropertyName("sortOrder")]
    public int SortOrder { get; set; }
}

/// <summary>Nhóm món.</summary>
public sealed class FnbMenuCategory
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }
}

/// <summary>Combo đang bán.</summary>
public sealed class FnbMenuCombo
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>Loại combo.</summary>
    [JsonPropertyName("comboType")]
    public ComboType ComboType { get; set; }

    /// <summary>Cách tính giá combo.</summary>
    [JsonPropertyName("pricingMode")]
    public ComboPricingMode PricingMode { get; set; }

    /// <summary>Mô tả.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>Giá đã gồm VAT.</summary>
    [JsonPropertyName("priceIncludeVAT")]
    public decimal PriceIncludeVAT { get; set; }

    /// <summary>Giá combo (chưa gồm VAT).</summary>
    [JsonPropertyName("priceCombo")]
    public decimal PriceCombo { get; set; }

    /// <summary>Giá combo đã gồm VAT.</summary>
    [JsonPropertyName("priceIncludeVATCombo")]
    public decimal PriceIncludeVATCombo { get; set; }

    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Mã ngành nghề tính thuế.</summary>
    [JsonPropertyName("taxIndustryCategoryId")]
    public string? TaxIndustryCategoryId { get; set; }

    /// <summary>Tổng số lần Combo được phép sử dụng.</summary>
    [JsonPropertyName("usageLimit")]
    public int? UsageLimit { get; set; }

    /// <summary>Thời hạn sử dụng Combo tính từ ngày mua (đơn vị: ngày).</summary>
    [JsonPropertyName("expiredInDays")]
    public int? ExpiredInDays { get; set; }

    /// <summary>Các nhóm.</summary>
    [JsonPropertyName("groups")]
    public List<FnbMenuComboGroup> Groups { get; set; } = new();

    /// <summary>Toàn bộ sản phẩm thuộc combo (gộp từ các nhóm).</summary>
    [JsonPropertyName("allProducts")]
    public List<FnbMenuComboProduct> AllProducts { get; set; } = new();
}

/// <summary>Bàn và đơn đang sử dụng.</summary>
public sealed class FnbTable
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Đơn đang sử dụng bàn.</summary>
    [JsonPropertyName("order")]
    public FnbOrderSummary? Order { get; set; }

    /// <summary>ID lịch đặt bàn gần nhất.</summary>
    [JsonPropertyName("nextBookingId")]
    public Guid? NextBookingId { get; set; }

    /// <summary>Thời điểm bắt đầu lịch đặt bàn gần nhất.</summary>
    [JsonPropertyName("nextBookingStart")]
    public DateTime? NextBookingStart { get; set; }

    /// <summary>Tên khách của lịch đặt bàn gần nhất.</summary>
    [JsonPropertyName("nextBookingCustomerName")]
    public string? NextBookingCustomerName { get; set; }

    /// <summary>Số điện thoại khách của lịch đặt bàn gần nhất.</summary>
    [JsonPropertyName("nextBookingCustomerPhone")]
    public string? NextBookingCustomerPhone { get; set; }

    /// <summary>Lịch đặt bàn sắp tới.</summary>
    [JsonPropertyName("upcomingBookings")]
    public List<FnbTableBooking> UpcomingBookings { get; set; } = new();
}

/// <summary>Nhóm lựa chọn của combo đang bán.</summary>
public sealed class FnbMenuComboGroup
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Số món cần chọn trong nhóm.</summary>
    [JsonPropertyName("requiredQuantity")]
    public int RequiredQuantity { get; set; }

    /// <summary>Danh sách sản phẩm.</summary>
    [JsonPropertyName("products")]
    public List<FnbMenuComboProduct> Products { get; set; } = new();
}

/// <summary>Sản phẩm của combo đang bán.</summary>
public sealed class FnbMenuComboProduct
{
    /// <summary>ID sản phẩm.</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>ID nhóm sản phẩm.</summary>
    [JsonPropertyName("categoryId")]
    public Guid CategoryId { get; set; }

    /// <summary>Có quản lý tồn kho.</summary>
    [JsonPropertyName("isInventory")]
    public bool IsInventory { get; set; }

    /// <summary>ID nhóm.</summary>
    [JsonPropertyName("groupId")]
    public Guid? GroupId { get; set; }

    /// <summary>Mã.</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Số lượng.</summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    /// <summary>ID đơn vị tính; <c>null</c> hoặc GUID rỗng là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }

    /// <summary>Tên đơn vị tính.</summary>
    [JsonPropertyName("unitName")]
    public string? UnitName { get; set; }

    /// <summary>Kiểu in của món.</summary>
    [JsonPropertyName("printType")]
    public ProductPrintType PrintType { get; set; }

    /// <summary>Tổng tiền.</summary>
    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    /// <summary>Tổng tiền đã gồm VAT.</summary>
    [JsonPropertyName("totalPriceIncludeVAT")]
    public decimal TotalPriceIncludeVAT { get; set; }
}

/// <summary>Lịch đặt bàn sắp tới.</summary>
public sealed class FnbTableBooking
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Bắt đầu.</summary>
    [JsonPropertyName("start")]
    public DateTime Start { get; set; }

    /// <summary>Kết thúc.</summary>
    [JsonPropertyName("end")]
    public DateTime? End { get; set; }

    /// <summary>Tên khách hàng.</summary>
    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    /// <summary>Số điện thoại khách hàng.</summary>
    [JsonPropertyName("customerPhone")]
    public string? CustomerPhone { get; set; }

    /// <summary>Tiền đặt cọc.</summary>
    [JsonPropertyName("depositAmount")]
    public decimal DepositAmount { get; set; }
}
