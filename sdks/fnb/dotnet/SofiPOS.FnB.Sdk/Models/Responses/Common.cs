using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Cặp ID và tên.</summary>
public sealed class IdTitle
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>Tiêu đề.</summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }
}

/// <summary>Khung giờ trong ngày.</summary>
public sealed class TimeSlot
{
    /// <summary>Giờ bắt đầu.</summary>
    [JsonPropertyName("startTime")]
    public TimeOnly StartTime { get; set; }

    /// <summary>Giờ kết thúc.</summary>
    [JsonPropertyName("endTime")]
    public TimeOnly EndTime { get; set; }
}
