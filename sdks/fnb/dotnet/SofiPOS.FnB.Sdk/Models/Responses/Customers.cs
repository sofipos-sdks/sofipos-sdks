using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Khách hàng trong danh sách.</summary>
public sealed class CustomerSummary
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tên khách hàng.</summary>
    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    /// <summary>Số điện thoại khách hàng.</summary>
    [JsonPropertyName("customerPhone")]
    public string? CustomerPhone { get; set; }

    /// <summary>Địa chỉ khách hàng.</summary>
    [JsonPropertyName("customerAddress")]
    public string? CustomerAddress { get; set; }

    /// <summary>Email khách hàng.</summary>
    [JsonPropertyName("customerEmail")]
    public string? CustomerEmail { get; set; }

    /// <summary>Số đơn hàng.</summary>
    [JsonPropertyName("orderCount")]
    public int OrderCount { get; set; }

    /// <summary>Thời điểm mua gần nhất.</summary>
    [JsonPropertyName("lastOrder")]
    public DateTime? LastOrder { get; set; }

    /// <summary>Tổng phải thu (đã gồm VAT, làm tròn đồng).</summary>
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    /// <summary>Điểm hiện có.</summary>
    [JsonPropertyName("currentPoint")]
    public int CurrentPoint { get; set; }

    /// <summary>Điểm cao nhất từng đạt.</summary>
    [JsonPropertyName("maxPointEver")]
    public int MaxPointEver { get; set; }

    /// <summary>Điểm cao nhất trong kỳ.</summary>
    [JsonPropertyName("maxPeakPointInPeriod")]
    public int MaxPeakPointInPeriod { get; set; }

    /// <summary>Hạng thành viên hiện tại.</summary>
    [JsonPropertyName("rank")]
    public MembershipRank? Rank { get; set; }
}

/// <summary>Chi tiết khách hàng.</summary>
public sealed class Customer
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tên khách hàng.</summary>
    [JsonPropertyName("customerName")]
    public string? CustomerName { get; set; }

    /// <summary>Số điện thoại khách hàng.</summary>
    [JsonPropertyName("customerPhone")]
    public string? CustomerPhone { get; set; }

    /// <summary>Địa chỉ khách hàng.</summary>
    [JsonPropertyName("customerAddress")]
    public string? CustomerAddress { get; set; }

    /// <summary>Số CCCD của khách hàng.</summary>
    [JsonPropertyName("customerCccd")]
    public string? CustomerCccd { get; set; }

    /// <summary>Giới tính: <c>true</c> nam, <c>false</c> nữ, <c>null</c> chưa xác định.</summary>
    [JsonPropertyName("gender")]
    public bool? Gender { get; set; }

    /// <summary>Ngày sinh.</summary>
    [JsonPropertyName("birthDate")]
    public DateTime? BirthDate { get; set; }

    /// <summary>Hạng thành viên hiện tại.</summary>
    [JsonPropertyName("rank")]
    public MembershipRank? Rank { get; set; }
}
