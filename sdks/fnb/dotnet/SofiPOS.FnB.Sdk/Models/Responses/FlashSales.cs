using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Chi tiết flash sale.</summary>
public sealed class FlashSale
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Ngày bắt đầu.</summary>
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>Ngày kết thúc; <c>null</c> là không giới hạn.</summary>
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Mô tả.</summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>Danh sách sản phẩm.</summary>
    [JsonPropertyName("products")]
    public List<FlashSaleProduct> Products { get; set; } = new();

    /// <summary>Danh sách ID chi nhánh áp dụng; rỗng là tất cả chi nhánh.</summary>
    [JsonPropertyName("branchIds")]
    public List<Guid> BranchIds { get; set; } = new();

    /// <summary>Kênh bán áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("saleChannelIds")]
    public List<string> SaleChannelIds { get; set; } = new();

    /// <summary>Đã phát sinh đơn hàng.</summary>
    [JsonPropertyName("hasBeenOrdered")]
    public bool HasBeenOrdered { get; set; }
}

/// <summary>Sản phẩm trong flash sale.</summary>
public sealed class FlashSaleProduct
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>ID sản phẩm.</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>Tên sản phẩm.</summary>
    [JsonPropertyName("productTitle")]
    public string? ProductTitle { get; set; }

    /// <summary>Mã sản phẩm.</summary>
    [JsonPropertyName("productCode")]
    public string? ProductCode { get; set; }

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Giá đã gồm VAT.</summary>
    [JsonPropertyName("priceIncludeVAT")]
    public decimal PriceIncludeVAT { get; set; }

    /// <summary>Tên đơn vị tính.</summary>
    [JsonPropertyName("unitName")]
    public string? UnitName { get; set; }

    /// <summary>ID đơn vị tính; <c>null</c> hoặc GUID rỗng là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }
}
