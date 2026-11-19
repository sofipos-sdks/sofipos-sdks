using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Kênh bán dùng được cho mã QR đặt món.</summary>
public sealed class QrOrderSaleChannel
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}

/// <summary>Thuộc tính đơn hàng của cửa hàng.</summary>
public sealed class OrderAttribute
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>Loại.</summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }

    /// <summary>Các giá trị có thể chọn.</summary>
    [JsonPropertyName("values")]
    public List<string>? Values { get; set; }

    /// <summary>Nhóm món.</summary>
    [JsonPropertyName("categories")]
    public List<IdTitle> Categories { get; set; } = new();
}

/// <summary>Mã QR đặt món.</summary>
public sealed class QrOrderCode
{
    /// <summary>ID.</summary>
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    /// <summary>ID chi nhánh.</summary>
    [JsonPropertyName("branchId")]
    public Guid BranchId { get; set; }

    /// <summary>Tên chi nhánh.</summary>
    [JsonPropertyName("branchName")]
    public string BranchName { get; set; } = string.Empty;

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>ID kênh bán (ví dụ <c>TaiCho</c>).</summary>
    [JsonPropertyName("saleChannelId")]
    public string SaleChannelId { get; set; } = string.Empty;

    /// <summary>Tên kênh bán.</summary>
    [JsonPropertyName("saleChannelName")]
    public string SaleChannelName { get; set; } = string.Empty;

    /// <summary>ID bàn.</summary>
    [JsonPropertyName("tableId")]
    public Guid? TableId { get; set; }

    /// <summary>Tên bàn.</summary>
    [JsonPropertyName("tableName")]
    public string? TableName { get; set; }

    /// <summary>Tên khu vực của bàn.</summary>
    [JsonPropertyName("areaName")]
    public string? AreaName { get; set; }

    /// <summary>Mã định danh của QR trong đường dẫn đặt món.</summary>
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>Null = không hết hạn.</summary>
    [JsonPropertyName("durationMinutes")]
    public int? DurationMinutes { get; set; }

    /// <summary>Ngày bắt đầu.</summary>
    [JsonPropertyName("startDate")]
    public DateTime StartDate { get; set; }

    /// <summary>Null = không hết hạn.</summary>
    [JsonPropertyName("expireDate")]
    public DateTime? ExpireDate { get; set; }

    /// <summary>Không giới hạn thời gian.</summary>
    [JsonPropertyName("noExpiry")]
    public bool NoExpiry { get; set; }

    /// <summary>Đang hoạt động.</summary>
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }

    /// <summary>Đã hết hạn.</summary>
    [JsonPropertyName("isExpired")]
    public bool IsExpired { get; set; }

    /// <summary>Bắt buộc khách nhập tên và số điện thoại.</summary>
    [JsonPropertyName("requireCustomerInfo")]
    public bool RequireCustomerInfo { get; set; }

    /// <summary>Chỉ dùng 1 lần (chỉ tạo được 1 đơn).</summary>
    [JsonPropertyName("isSingleUse")]
    public bool IsSingleUse { get; set; }

    /// <summary>QR dùng 1 lần đã có đơn.</summary>
    [JsonPropertyName("isUsed")]
    public bool IsUsed { get; set; }

    /// <summary>Danh sách thuộc tính.</summary>
    [JsonPropertyName("attributes")]
    public List<QrOrderCodeAttribute> Attributes { get; set; } = new();

    /// <summary>Số đơn hàng.</summary>
    [JsonPropertyName("orderCount")]
    public int OrderCount { get; set; }

    /// <summary>Đường dẫn trang đặt món tương đối (qr-order/{index}/{token}).</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>Thời điểm tạo.</summary>
    [JsonPropertyName("createDate")]
    public DateTime CreateDate { get; set; }
}

/// <summary>Đường dẫn và ảnh mã QR đặt món.</summary>
public sealed class QrOrderCodeImage
{
    /// <summary>Đường dẫn.</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>Ảnh mã QR (PNG base64).</summary>
    [JsonPropertyName("qrCodeBase64")]
    public string QrCodeBase64 { get; set; } = string.Empty;
}

/// <summary>Thuộc tính đơn khách cần nhập khi đặt món qua QR.</summary>
public sealed class QrOrderCodeAttribute
{
    /// <summary>ID thuộc tính đơn hàng.</summary>
    [JsonPropertyName("attributeId")]
    public string AttributeId { get; set; } = string.Empty;

    /// <summary>Tên.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Loại.</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>Bắt buộc.</summary>
    [JsonPropertyName("isRequired")]
    public bool IsRequired { get; set; }

    /// <summary>Thứ tự ưu tiên.</summary>
    [JsonPropertyName("priority")]
    public int Priority { get; set; }
}
