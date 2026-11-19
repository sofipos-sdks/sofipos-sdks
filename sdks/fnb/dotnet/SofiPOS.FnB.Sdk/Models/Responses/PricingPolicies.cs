using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Chính sách giá trong danh sách.</summary>
public sealed class PricingPolicySummary
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Ngày bắt đầu.</summary>
    [JsonPropertyName("fromDate")]
    public DateTime? FromDate { get; set; }

    /// <summary>Ngày kết thúc.</summary>
    [JsonPropertyName("toDate")]
    public DateTime? ToDate { get; set; }

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }

    /// <summary>Thời điểm cập nhật.</summary>
    [JsonPropertyName("updateDate")]
    public DateTime UpdateDate { get; set; }

    /// <summary>Danh sách ID chi nhánh áp dụng; rỗng là tất cả chi nhánh.</summary>
    [JsonPropertyName("branchIds")]
    public List<Guid> BranchIds { get; set; } = new();
}

/// <summary>Chi tiết chính sách giá sản phẩm.</summary>
public sealed class PricingPolicy
{
    /// <summary>Kênh bán áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("saleChannelIds")]
    public List<string> SaleChannelIds { get; set; } = new();

    /// <summary>Các ngày trong tuần áp dụng; rỗng là tất cả các ngày.</summary>
    [JsonPropertyName("daysOfWeek")]
    public List<DayOfWeek> DaysOfWeek { get; set; } = new();

    /// <summary>Danh sách sản phẩm.</summary>
    [JsonPropertyName("products")]
    public List<PricingPolicyProduct> Products { get; set; } = new();

    /// <summary>Khung giờ áp dụng; rỗng là cả ngày.</summary>
    [JsonPropertyName("timeSlots")]
    public List<TimeSlot> TimeSlots { get; set; } = new();

    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Ngày bắt đầu.</summary>
    [JsonPropertyName("fromDate")]
    public DateTime? FromDate { get; set; }

    /// <summary>Ngày kết thúc.</summary>
    [JsonPropertyName("toDate")]
    public DateTime? ToDate { get; set; }

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }

    /// <summary>Thời điểm cập nhật.</summary>
    [JsonPropertyName("updateDate")]
    public DateTime UpdateDate { get; set; }

    /// <summary>Danh sách ID chi nhánh áp dụng; rỗng là tất cả chi nhánh.</summary>
    [JsonPropertyName("branchIds")]
    public List<Guid> BranchIds { get; set; } = new();
}

/// <summary>Món kèm hợp lệ của sản phẩm chính.</summary>
public sealed class PricingPolicyAddOnOption
{
    /// <summary>ID sản phẩm chính.</summary>
    [JsonPropertyName("parentProductId")]
    public Guid ParentProductId { get; set; }

    /// <summary>ID sản phẩm món kèm.</summary>
    [JsonPropertyName("addOnProductId")]
    public Guid AddOnProductId { get; set; }

    /// <summary>ID đơn vị tính của món kèm; <c>null</c> là đơn vị cơ bản.</summary>
    [JsonPropertyName("addOnUnitId")]
    public Guid? AddOnUnitId { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Mã.</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>Tên đơn vị tính.</summary>
    [JsonPropertyName("unitName")]
    public string? UnitName { get; set; }

    /// <summary>Thuế suất VAT (%).</summary>
    [JsonPropertyName("vat")]
    public int VAT { get; set; }

    /// <summary>Giá dùng khi chính sách không đặt giá món kèm.</summary>
    [JsonPropertyName("defaultPrice")]
    public decimal DefaultPrice { get; set; }
}

/// <summary>Giá sản phẩm trong chính sách giá.</summary>
public sealed class PricingPolicyProduct
{
    /// <summary>ID sản phẩm.</summary>
    [JsonPropertyName("productId")]
    public Guid ProductId { get; set; }

    /// <summary>ID đơn vị tính; <c>null</c> hoặc GUID rỗng là đơn vị cơ bản.</summary>
    [JsonPropertyName("unitId")]
    public Guid? UnitId { get; set; }

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    /// <summary>Danh sách món kèm.</summary>
    [JsonPropertyName("addOns")]
    public List<PricingPolicyProductAddOn>? AddOns { get; set; }
}

/// <summary>Giá món kèm trong chính sách giá.</summary>
public sealed class PricingPolicyProductAddOn
{
    /// <summary>ID sản phẩm món kèm.</summary>
    [JsonPropertyName("addOnProductId")]
    public Guid AddOnProductId { get; set; }

    /// <summary>ID đơn vị tính của món kèm; <c>null</c> là đơn vị cơ bản.</summary>
    [JsonPropertyName("addOnUnitId")]
    public Guid? AddOnUnitId { get; set; }

    /// <summary>Giá (chưa gồm VAT nếu có trường VAT đi kèm).</summary>
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}
