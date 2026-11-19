using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Hạng thành viên của cửa hàng.</summary>
public sealed class MembershipRank
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tên hạng thành viên.</summary>
    [JsonPropertyName("rankName")]
    public string? RankName { get; set; }

    /// <summary>Ngưỡng điểm đạt hạng.</summary>
    [JsonPropertyName("thresholdPoint")]
    public int ThresholdPoint { get; set; }

    /// <summary>Mô tả quyền lợi của hạng.</summary>
    [JsonPropertyName("benefits")]
    public string? Benefits { get; set; }

    /// <summary>Tỷ lệ tích điểm.</summary>
    [JsonPropertyName("earnRate")]
    public decimal EarnRate { get; set; }

    /// <summary>Quy tắc làm tròn điểm tích lũy.</summary>
    [JsonPropertyName("earnRounding")]
    public PointEarnRoundingRule EarnRounding { get; set; }

    /// <summary>Tỷ lệ quy đổi điểm thành tiền.</summary>
    [JsonPropertyName("burnRate")]
    public decimal BurnRate { get; set; }

    /// <summary>Số tiền tối đa được trừ bằng điểm mỗi đơn.</summary>
    [JsonPropertyName("maxBurnAmountPerOrder")]
    public decimal MaxBurnAmountPerOrder { get; set; }

    /// <summary>Tỷ lệ % tối đa được trừ bằng điểm mỗi đơn.</summary>
    [JsonPropertyName("maxBurnPercentPerOrder")]
    public decimal MaxBurnPercentPerOrder { get; set; }

    /// <summary>Trừ điểm trên giá trị trước hay sau VAT.</summary>
    [JsonPropertyName("pointBurnTaxBasis")]
    public PointBurnTaxBasis PointBurnTaxBasis { get; set; }
}

/// <summary>Chi nhánh của cửa hàng.</summary>
public sealed class Branch
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}
