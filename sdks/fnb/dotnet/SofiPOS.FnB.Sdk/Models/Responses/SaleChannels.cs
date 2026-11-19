using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Kênh bán trong danh mục hệ thống.</summary>
public sealed class SaleChannelOption
{
    /// <summary>ID kênh bán (ví dụ <c>TaiCho</c>).</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Mã ngôn ngữ của tên kênh.</summary>
    [JsonPropertyName("languageCode")]
    public string? LanguageCode { get; set; }

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }
}

/// <summary>Kênh bán đã cấu hình cho cửa hàng.</summary>
public sealed class SaleChannel
{
    /// <summary>ID kênh bán (ví dụ <c>TaiCho</c>).</summary>
    [JsonPropertyName("saleChannelId")]
    public string? SaleChannelId { get; set; }

    /// <summary>Tên kênh bán.</summary>
    [JsonPropertyName("saleChannelName")]
    public string? SaleChannelName { get; set; }

    /// <summary>ID cửa hàng.</summary>
    [JsonPropertyName("companyId")]
    public Guid CompanyId { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Có thể xóa (kênh chưa phát sinh đơn hàng).</summary>
    [JsonPropertyName("canDelete")]
    public bool CanDelete { get; set; }

    /// <summary>Loại kênh bán.</summary>
    [JsonPropertyName("isECommerce")]
    public SaleChannelECommerceType IsECommerce { get; set; }
}
