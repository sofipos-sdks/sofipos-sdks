using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Voucher trong danh sách.</summary>
public sealed class VoucherSummary
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Ngày bắt đầu.</summary>
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>Ngày kết thúc; <c>null</c> là không giới hạn.</summary>
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Mô tả ngắn.</summary>
    [JsonPropertyName("brief")]
    public string? Brief { get; set; }

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>Giá trị giảm.</summary>
    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    /// <summary>Kiểu giảm giá.</summary>
    [JsonPropertyName("discountType")]
    public DiscountType DiscountType { get; set; }

    /// <summary>Số lượng mã voucher.</summary>
    [JsonPropertyName("voucherNumber")]
    public int VoucherNumber { get; set; }

    /// <summary>Áp dụng voucher trước thuế (true) hay sau thuế (false) true: Áp dụng trước thuế false: Áp dụng sau thuế.</summary>
    [JsonPropertyName("isApplyBeforeVAT")]
    public bool IsApplyBeforeVAT { get; set; }

    /// <summary>Trạng thái hiển thị.</summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>Chi tiết voucher.</summary>
public sealed class Voucher
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Ngày bắt đầu.</summary>
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>Ngày kết thúc; <c>null</c> là không giới hạn.</summary>
    [JsonPropertyName("endDate")]
    public DateTime? EndDate { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>Mô tả ngắn.</summary>
    [JsonPropertyName("brief")]
    public string? Brief { get; set; }

    /// <summary>Nội dung chi tiết.</summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>Ảnh.</summary>
    [JsonPropertyName("image")]
    public ImageFile? Image { get; set; }

    /// <summary>Giá trị giảm.</summary>
    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }

    /// <summary>Kiểu giảm giá.</summary>
    [JsonPropertyName("discountType")]
    public DiscountType DiscountType { get; set; }

    /// <summary>Số lần sử dụng tối đa.</summary>
    [JsonPropertyName("usageLimit")]
    public int UsageLimit { get; set; }

    /// <summary>Giá trị đơn tối thiểu; 0 là không giới hạn.</summary>
    [JsonPropertyName("min")]
    public decimal Min { get; set; }

    /// <summary>Số tiền giảm tối đa; 0 là không giới hạn.</summary>
    [JsonPropertyName("max")]
    public decimal Max { get; set; }

    /// <summary>Áp dụng voucher trước thuế (true) hay sau thuế (false) true: Áp dụng trước thuế false: Áp dụng sau thuế.</summary>
    [JsonPropertyName("isApplyBeforeVAT")]
    public bool IsApplyBeforeVAT { get; set; }

    /// <summary>Tổng số mã voucher đã phát sinh.</summary>
    [JsonPropertyName("totalVoucherCount")]
    public int TotalVoucherCount { get; set; }

    /// <summary>Có ít nhất một mã voucher đã dùng trong đơn — không cho phép sửa thông tin chi tiết qua API cập nhật đầy đủ.</summary>
    [JsonPropertyName("hasUsedCodes")]
    public bool HasUsedCodes { get; set; }
}

/// <summary>Mã voucher hoặc một lần sử dụng mã.</summary>
public sealed class VoucherCode
{
    /// <summary>Mã voucher.</summary>
    [JsonPropertyName("voucherCode")]
    public string? VoucherCodeValue { get; set; }

    /// <summary>Mã đơn hàng.</summary>
    [JsonPropertyName("orderCode")]
    public string? OrderCode { get; set; }

    /// <summary>Thời điểm sử dụng.</summary>
    [JsonPropertyName("usedDate")]
    public DateTime? UsedDate { get; set; }

    /// <summary>Số lần mã đã được áp dụng trong đơn hàng.</summary>
    [JsonPropertyName("usedCount")]
    public int UsedCount { get; set; }

    /// <summary>Số lần còn lại theo giới hạn chương trình. Null = không giới hạn (UsageLimit = 0).</summary>
    [JsonPropertyName("remainingUses")]
    public int? RemainingUses { get; set; }
}
