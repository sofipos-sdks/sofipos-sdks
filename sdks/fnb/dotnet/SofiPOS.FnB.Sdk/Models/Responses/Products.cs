using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Sản phẩm POS của cửa hàng.</summary>
public sealed class Product
{
    /// <summary>ID sản phẩm.</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>Mã sản phẩm.</summary>
    [JsonPropertyName("productCode")]
    public string ProductCode { get; set; } = string.Empty;

    /// <summary>Tên sản phẩm.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Giá bán đã gồm VAT.</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>Đơn vị tính cơ bản.</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>Nhóm sản phẩm hiện tại.</summary>
    [JsonPropertyName("categoryId")]
    public Guid? CategoryId { get; set; }

    /// <summary>Thời điểm cập nhật gần nhất trên POS.</summary>
    [JsonPropertyName("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}

/// <summary>Đơn vị tính của sản phẩm. Phần tử đầu là đơn vị cơ bản (<c>Id</c> là GUID rỗng).</summary>
public sealed class ProductUnit
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tên đơn vị tính.</summary>
    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    /// <summary>Tỷ lệ quy đổi so với đơn vị cơ bản.</summary>
    [JsonPropertyName("convert")]
    public decimal Convert { get; set; }

    /// <summary>Giá bán của đơn vị tính.</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>Cấu hình làm tròn.</summary>
    [JsonPropertyName("rounding")]
    public ProductUnitRounding? Rounding { get; set; }

    /// <summary>Bảng giá theo bậc số lượng.</summary>
    [JsonPropertyName("rates")]
    public List<ProductUnitRate> Rates { get; set; } = new();
}

/// <summary>Cấu hình làm tròn của đơn vị tính.</summary>
public sealed class ProductUnitRounding
{
    /// <summary>ID đơn vị tính; <c>null</c> hoặc GUID rỗng là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid UnitId { get; set; }

    /// <summary>Vượt ngưỡng bao nhiêu thì làm tròn lên.</summary>
    [JsonPropertyName("overToUp")]
    public int OverToUp { get; set; }

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}

/// <summary>Giá theo bậc số lượng của đơn vị tính.</summary>
public sealed class ProductUnitRate
{
    /// <summary>ID đơn vị tính; <c>null</c> hoặc GUID rỗng là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid UnitId { get; set; }

    /// <summary>Bắt đầu.</summary>
    [JsonPropertyName("start")]
    public int Start { get; set; }

    /// <summary>Kết thúc.</summary>
    [JsonPropertyName("end")]
    public int End { get; set; }

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}
