using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Một trang dữ liệu của API danh sách.</summary>
/// <typeparam name="T">Kiểu phần tử.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>Thông tin phân trang.</summary>
    [JsonPropertyName("metaData")]
    public PageMetadata MetaData { get; set; } = new();

    /// <summary>Các phần tử của trang hiện tại.</summary>
    [JsonPropertyName("items")]
    public List<T> Items { get; set; } = new();
}

/// <summary>Thông tin phân trang do API trả về.</summary>
public sealed class PageMetadata
{
    /// <summary>Trang hiện tại (bắt đầu từ 1).</summary>
    [JsonPropertyName("currentPage")]
    public int CurrentPage { get; set; }

    /// <summary>Tổng số trang.</summary>
    [JsonPropertyName("totalPages")]
    public int TotalPages { get; set; }

    /// <summary>Số phần tử mỗi trang.</summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    /// <summary>Tổng số phần tử.</summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    /// <summary>Có trang trước.</summary>
    [JsonPropertyName("hasPrevious")]
    public bool HasPrevious { get; set; }

    /// <summary>Có trang sau.</summary>
    [JsonPropertyName("hasNext")]
    public bool HasNext { get; set; }
}

/// <summary>Tham số phân trang chung của API danh sách.</summary>
public class PageQuery
{
    /// <summary>Trang cần lấy, bắt đầu từ 1. Mặc định 1.</summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>Số phần tử mỗi trang, nên từ 1 đến 100. Mặc định 15.</summary>
    public int PageSize { get; set; } = 15;
}
