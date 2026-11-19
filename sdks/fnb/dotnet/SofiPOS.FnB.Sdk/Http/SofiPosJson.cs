using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SofiPOS.FnB.Http;

/// <summary>Cấu hình JSON dùng chung của SDK, khớp quy ước của SofiPOS Partner API.</summary>
public static class SofiPosJson
{
    /// <summary>
    /// Tùy chọn System.Text.Json: tên trường camelCase, enum dạng số, bỏ trường <c>null</c> khi gửi,
    /// thời gian theo giờ Việt Nam (xem <see cref="VietnamDateTimeConverter"/>).
    /// </summary>
    public static JsonSerializerOptions Options { get; } = CreateOptions();

    private static JsonSerializerOptions CreateOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        options.Converters.Add(new VietnamDateTimeConverter());
        options.MakeReadOnly(populateMissingResolver: true);
        return options;
    }
}

/// <summary>
/// Thời gian của SofiPOS là giờ Việt Nam (GMT+7). Khi đọc, giá trị có hậu tố múi giờ (ví dụ <c>+07:00</c>, <c>Z</c>)
/// được quy về giờ Việt Nam; giá trị không có múi giờ giữ nguyên. Kết quả luôn có <see cref="DateTimeKind.Unspecified"/>
/// để không bị đổi theo múi giờ của máy đối tác. Khi gửi, ghi dạng <c>yyyy-MM-ddTHH:mm:ss</c>.
/// </summary>
public sealed class VietnamDateTimeConverter : JsonConverter<DateTime>
{
    /// <summary>Độ lệch giờ Việt Nam so với UTC.</summary>
    public static readonly TimeSpan VietnamOffset = TimeSpan.FromHours(7);

    /// <inheritdoc />
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException("Thời gian phải là chuỗi ISO 8601.");
        }

        var text = reader.GetString();
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new JsonException("Thời gian rỗng.");
        }

        return Parse(text);
    }

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        if (value.Kind == DateTimeKind.Utc)
        {
            value = DateTime.SpecifyKind(value + VietnamOffset, DateTimeKind.Unspecified);
        }

        writer.WriteStringValue(value.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture));
    }

    /// <summary>Đọc chuỗi ISO 8601 thành giờ Việt Nam.</summary>
    public static DateTime Parse(string text)
    {
        text = text.Trim();
        if (HasOffset(text))
        {
            var withOffset = DateTimeOffset.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            return DateTime.SpecifyKind(withOffset.ToOffset(VietnamOffset).DateTime, DateTimeKind.Unspecified);
        }

        var local = DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces);
        return DateTime.SpecifyKind(local, DateTimeKind.Unspecified);
    }

    private static bool HasOffset(string text)
    {
        if (text.EndsWith("Z", StringComparison.OrdinalIgnoreCase)) return true;
        var timeStart = text.IndexOf('T');
        if (timeStart < 0) return false;
        return text.IndexOf('+', timeStart) > 0 || text.IndexOf('-', timeStart) > 0;
    }
}
