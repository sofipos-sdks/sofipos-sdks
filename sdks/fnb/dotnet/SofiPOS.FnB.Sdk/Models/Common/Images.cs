using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Models;

/// <summary>Ảnh đã lưu trên SofiPOS.</summary>
public sealed class ImageFile
{
    /// <summary>Tên file đã lưu.</summary>
    [JsonPropertyName("fileName")]
    public string? FileName { get; set; }

    /// <summary>Thư mục chứa ảnh (đường dẫn tương đối).</summary>
    [JsonPropertyName("filePath")]
    public string? FilePath { get; set; }

    /// <summary>Đường dẫn tương đối của ảnh.</summary>
    [JsonPropertyName("fullPath")]
    public string? FullPath { get; set; }
}

/// <summary>
/// Ảnh gửi lên khi tạo/sửa combo, flash sale, voucher. Hỗ trợ <c>.jpg</c>, <c>.jpeg</c>, <c>.png</c>, <c>.webp</c>,
/// <c>.gif</c>, <c>.bmp</c>. Không gửi ảnh thì giữ ảnh hiện tại.
/// </summary>
public sealed class ImageUpload
{
    /// <summary>Tên file kèm phần mở rộng, ví dụ <c>combo-sang.jpg</c>.</summary>
    [JsonPropertyName("fileName")]
    public string FileName { get; set; } = string.Empty;

    /// <summary>Nội dung ảnh dạng base64, không kèm tiền tố <c>data:</c>.</summary>
    [JsonPropertyName("base64data")]
    public string Base64Data { get; set; } = string.Empty;

    /// <summary>Tạo ảnh từ mảng byte.</summary>
    public static ImageUpload FromBytes(string fileName, ReadOnlySpan<byte> content)
        => new() { FileName = fileName, Base64Data = Convert.ToBase64String(content) };

    /// <summary>Đọc ảnh từ file trên đĩa.</summary>
    public static async Task<ImageUpload> FromFileAsync(string path, CancellationToken cancellationToken = default)
    {
        var bytes = await File.ReadAllBytesAsync(path, cancellationToken).ConfigureAwait(false);
        return FromBytes(Path.GetFileName(path), bytes);
    }
}
