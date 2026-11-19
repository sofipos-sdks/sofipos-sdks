using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Combo trong danh sách.</summary>
public sealed class ComboSummary
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Mô tả.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>Loại combo.</summary>
    [JsonPropertyName("comboType")]
    public ComboType ComboType { get; set; }

    /// <summary>Cách tính giá combo.</summary>
    [JsonPropertyName("pricingMode")]
    public ComboPricingMode PricingMode { get; set; }

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal? Price { get; set; }

    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int? VAT { get; set; }

    /// <summary>Các nhóm.</summary>
    [JsonPropertyName("groups")]
    public List<ComboGroup> Groups { get; set; } = new();
}

/// <summary>Chi tiết combo.</summary>
public sealed class Combo
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>ID chương trình khuyến mại chứa combo.</summary>
    [JsonPropertyName("campaignId")]
    public Guid CampaignId { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Đã phát sinh đơn hàng.</summary>
    [JsonPropertyName("hasBeenOrdered")]
    public bool HasBeenOrdered { get; set; }

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

    /// <summary>Số lần sử dụng tối đa (chỉ áp dụng cho Service). 1 = combo thường, > 1 = combo nhiều lần.</summary>
    [JsonPropertyName("usageLimit")]
    public int? UsageLimit { get; set; }

    /// <summary>Thời hạn sử dụng tính từ ngày mua, đơn vị: ngày (chỉ áp dụng cho Service).</summary>
    [JsonPropertyName("expiredInDays")]
    public int? ExpiredInDays { get; set; }

    /// <summary>Giá đã gồm VAT.</summary>
    [JsonPropertyName("priceIncludeVAT")]
    public decimal PriceIncludeVAT { get; set; }

    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Mã ngành nghề tính thuế.</summary>
    [JsonPropertyName("taxIndustryCategoryId")]
    public string? TaxIndustryCategoryId { get; set; }

    /// <summary>Các nhóm.</summary>
    [JsonPropertyName("groups")]
    public List<ComboGroup> Groups { get; set; } = new();

    /// <summary>Sản phẩm của combo cố định.</summary>
    [JsonPropertyName("fixedProducts")]
    public List<ComboProduct> FixedProducts { get; set; } = new();

    /// <summary>Tổng tiền.</summary>
    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    /// <summary>Tổng tiền đã gồm VAT.</summary>
    [JsonPropertyName("totalPriceIncludeVAT")]
    public decimal TotalPriceIncludeVAT { get; set; }
}

/// <summary>Nhóm lựa chọn của combo tự chọn.</summary>
public sealed class ComboGroup
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
    public List<ComboProduct> Products { get; set; } = new();
}

/// <summary>Sản phẩm trong combo.</summary>
public sealed class ComboProduct
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>ID sản phẩm.</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>ID nhóm.</summary>
    [JsonPropertyName("groupId")]
    public Guid? GroupId { get; set; }

    /// <summary>Tổng tiền.</summary>
    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }

    /// <summary>Mã sản phẩm.</summary>
    [JsonPropertyName("productCode")]
    public string? ProductCode { get; set; }

    /// <summary>Tên sản phẩm.</summary>
    [JsonPropertyName("productTitle")]
    public string? ProductTitle { get; set; }

    /// <summary>Ảnh sản phẩm.</summary>
    [JsonPropertyName("productImage")]
    public ImageFile? ProductImage { get; set; }

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

    /// <summary>Tổng tiền đã gồm VAT.</summary>
    [JsonPropertyName("totalPriceIncludeVAT")]
    public decimal TotalPriceIncludeVAT { get; set; }
}
