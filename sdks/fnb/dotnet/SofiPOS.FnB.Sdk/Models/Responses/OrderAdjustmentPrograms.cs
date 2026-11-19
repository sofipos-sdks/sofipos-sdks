using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Chương trình chiết khấu/phụ thu trong danh sách.</summary>
public sealed class OrderAdjustmentProgramSummary
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Loại.</summary>
    [JsonPropertyName("type")]
    public OrderAdjustmentType Type { get; set; }

    /// <summary>Kiểu giá trị: theo % hoặc số tiền.</summary>
    [JsonPropertyName("valueType")]
    public DiscountType ValueType { get; set; }

    /// <summary>Giá trị.</summary>
    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    /// <summary>Áp dụng trên giá trị trước VAT.</summary>
    [JsonPropertyName("isApplyBeforeVAT")]
    public bool IsApplyBeforeVAT { get; set; }

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

    /// <summary>Phiên bản dữ liệu, gửi lại khi sửa để chống ghi đè.</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }

    /// <summary>Thời điểm cập nhật.</summary>
    [JsonPropertyName("updateDate")]
    public DateTime UpdateDate { get; set; }

    /// <summary>Danh sách ID chi nhánh áp dụng; rỗng là tất cả chi nhánh.</summary>
    [JsonPropertyName("branchIds")]
    public List<Guid> BranchIds { get; set; } = new();

    /// <summary>Hạng thành viên áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("membershipRankIds")]
    public List<Guid> MembershipRankIds { get; set; } = new();
}

/// <summary>Chi tiết chương trình chiết khấu/phụ thu theo đơn.</summary>
public sealed class OrderAdjustmentProgram
{
    /// <summary>Kênh bán áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("saleChannelIds")]
    public List<string> SaleChannelIds { get; set; } = new();

    /// <summary>Các ngày trong tuần áp dụng; rỗng là tất cả các ngày.</summary>
    [JsonPropertyName("daysOfWeek")]
    public List<DayOfWeek> DaysOfWeek { get; set; } = new();

    /// <summary>Khung giờ áp dụng; rỗng là cả ngày.</summary>
    [JsonPropertyName("timeSlots")]
    public List<TimeSlot> TimeSlots { get; set; } = new();

    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Loại.</summary>
    [JsonPropertyName("type")]
    public OrderAdjustmentType Type { get; set; }

    /// <summary>Kiểu giá trị: theo % hoặc số tiền.</summary>
    [JsonPropertyName("valueType")]
    public DiscountType ValueType { get; set; }

    /// <summary>Giá trị.</summary>
    [JsonPropertyName("value")]
    public decimal Value { get; set; }

    /// <summary>Áp dụng trên giá trị trước VAT.</summary>
    [JsonPropertyName("isApplyBeforeVAT")]
    public bool IsApplyBeforeVAT { get; set; }

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

    /// <summary>Phiên bản dữ liệu, gửi lại khi sửa để chống ghi đè.</summary>
    [JsonPropertyName("version")]
    public int Version { get; set; }

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }

    /// <summary>Thời điểm cập nhật.</summary>
    [JsonPropertyName("updateDate")]
    public DateTime UpdateDate { get; set; }

    /// <summary>Danh sách ID chi nhánh áp dụng; rỗng là tất cả chi nhánh.</summary>
    [JsonPropertyName("branchIds")]
    public List<Guid> BranchIds { get; set; } = new();

    /// <summary>Hạng thành viên áp dụng; rỗng là tất cả.</summary>
    [JsonPropertyName("membershipRankIds")]
    public List<Guid> MembershipRankIds { get; set; } = new();
}
