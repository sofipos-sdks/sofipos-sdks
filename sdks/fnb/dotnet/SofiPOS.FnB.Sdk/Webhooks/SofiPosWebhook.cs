using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using SofiPOS.FnB.Http;

namespace SofiPOS.FnB.Webhooks;

/// <summary>Header SofiPOS gửi kèm mỗi webhook.</summary>
public static class SofiPosWebhookHeaders
{
    /// <summary>ID sự kiện, giữ nguyên khi gửi lại; dùng để bỏ qua sự kiện trùng.</summary>
    public const string Id = "X-Webhook-Id";

    /// <summary>Tên sự kiện, ví dụ <c>order.created</c>.</summary>
    public const string Event = "X-Webhook-Event";

    /// <summary>Unix time (giây, UTC) lúc gửi, dùng trong chữ ký.</summary>
    public const string Timestamp = "X-Webhook-Timestamp";

    /// <summary>Chữ ký <c>sha256=&lt;hex&gt;</c>.</summary>
    public const string Signature = "X-Webhook-Signature";
}

/// <summary>Tên sự kiện webhook.</summary>
public static class SofiPosWebhookEvents
{
    /// <summary>Cửa hàng bấm Gửi thử.</summary>
    public const string Ping = "ping";

    /// <summary>Có đơn FnB mới.</summary>
    public const string OrderCreated = "order.created";

    /// <summary>Đơn FnB thay đổi (món, combo, khách, bàn, giảm giá, thanh toán, hủy...).</summary>
    public const string OrderUpdated = "order.updated";

    /// <summary>Sản phẩm, đơn vị tính, món kèm, giá theo kênh.</summary>
    public const string ProductChanged = "product.changed";

    /// <summary>Nhóm sản phẩm.</summary>
    public const string CategoryChanged = "category.changed";

    /// <summary>Combo.</summary>
    public const string ComboChanged = "combo.changed";

    /// <summary>Flash sale.</summary>
    public const string FlashSaleChanged = "flash_sale.changed";

    /// <summary>Chính sách giá.</summary>
    public const string PricingPolicyChanged = "pricing_policy.changed";

    /// <summary>Khách hàng.</summary>
    public const string CustomerChanged = "customer.changed";

    /// <summary>Voucher, mã voucher.</summary>
    public const string VoucherChanged = "voucher.changed";

    /// <summary>Chương trình chiết khấu/phụ thu.</summary>
    public const string OrderAdjustmentProgramChanged = "order_adjustment_program.changed";

    /// <summary>Kênh bán của cửa hàng (<c>ids</c> là <c>saleChannelId</c>).</summary>
    public const string SaleChannelChanged = "sale_channel.changed";

    /// <summary>Mã QR đặt món.</summary>
    public const string QrOrderCodeChanged = "qr_order_code.changed";

    /// <summary>Khu vực, bàn (<c>ids</c> là ID khu vực).</summary>
    public const string AreaTableChanged = "area_table.changed";

    /// <summary>Hạng thành viên.</summary>
    public const string MembershipRankChanged = "membership_rank.changed";
}

/// <summary>Một sự kiện webhook. Webhook chỉ báo có thay đổi; gọi API GET tương ứng để lấy dữ liệu mới nhất.</summary>
public sealed class SofiPosWebhookEvent
{
    /// <summary>ID sự kiện (bằng header <c>X-Webhook-Id</c>).</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>Tên sự kiện, xem <see cref="SofiPosWebhookEvents"/>.</summary>
    [JsonPropertyName("event")]
    public string Event { get; set; } = string.Empty;

    /// <summary>Thời điểm thay đổi được ghi nhận.</summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Dữ liệu sự kiện dạng JSON gốc.</summary>
    [JsonPropertyName("data")]
    public JsonElement Data { get; set; }

    /// <summary>Sự kiện đơn hàng (<c>order.created</c>, <c>order.updated</c>).</summary>
    [JsonIgnore]
    public bool IsOrderEvent => Event is SofiPosWebhookEvents.OrderCreated or SofiPosWebhookEvents.OrderUpdated;

    /// <summary>Đọc dữ liệu của sự kiện đơn hàng.</summary>
    public WebhookOrderData GetOrderData() => GetData<WebhookOrderData>();

    /// <summary>Đọc dữ liệu của sự kiện danh mục (<c>*.changed</c>).</summary>
    public WebhookChangeData GetChangeData() => GetData<WebhookChangeData>();

    /// <summary>Đọc dữ liệu của sự kiện <c>ping</c>.</summary>
    public WebhookPingData GetPingData() => GetData<WebhookPingData>();

    /// <summary>Đọc dữ liệu sự kiện thành kiểu tùy chọn.</summary>
    public T GetData<T>()
    {
        if (Data.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
        {
            throw new InvalidOperationException("Sự kiện webhook không có trường data.");
        }

        return Data.Deserialize<T>(SofiPosJson.Options) ?? throw new InvalidOperationException("Không đọc được data của sự kiện webhook.");
    }
}

/// <summary>Dữ liệu sự kiện đơn hàng.</summary>
public sealed class WebhookOrderData
{
    /// <summary>ID đơn, dùng gọi <c>FnbOrders.GetAsync</c>.</summary>
    [JsonPropertyName("orderId")]
    public Guid OrderId { get; set; }

    /// <summary>Mã đơn.</summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>Chi nhánh của đơn.</summary>
    [JsonPropertyName("branchId")]
    public Guid BranchId { get; set; }
}

/// <summary>Dữ liệu sự kiện danh mục.</summary>
public sealed class WebhookChangeData
{
    /// <summary>ID bản ghi được tạo hoặc sửa.</summary>
    [JsonPropertyName("ids")]
    public List<string> Ids { get; set; } = new();

    /// <summary>ID bản ghi đã xóa (gọi GET sẽ trả 404).</summary>
    [JsonPropertyName("deletedIds")]
    public List<string> DeletedIds { get; set; } = new();
}

/// <summary>Dữ liệu sự kiện <c>ping</c>.</summary>
public sealed class WebhookPingData
{
    /// <summary>ID đối tác.</summary>
    [JsonPropertyName("partnerId")]
    public Guid PartnerId { get; set; }

    /// <summary>Thông báo thử.</summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}

/// <summary>Webhook không hợp lệ: thiếu header, sai chữ ký, quá hạn hoặc body không đọc được.</summary>
public sealed class SofiPosWebhookException : Exception
{
    /// <summary>Tạo exception.</summary>
    public SofiPosWebhookException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}

/// <summary>
/// Kiểm tra chữ ký và đọc webhook của SofiPOS. Chữ ký:
/// <c>X-Webhook-Signature = "sha256=" + hex_lowercase(HMAC_SHA256(secret, X-Webhook-Timestamp + "." + rawBody))</c>.
/// Không phụ thuộc ASP.NET Core; dùng được với mọi web framework.
/// </summary>
public static class SofiPosWebhookVerifier
{
    /// <summary>Độ lệch thời gian cho phép mặc định (5 phút) để chống gửi lại request cũ.</summary>
    public static readonly TimeSpan DefaultTolerance = TimeSpan.FromMinutes(5);

    private const string SignaturePrefix = "sha256=";

    /// <summary>Tính chữ ký cho body dạng byte (nguyên văn body nhận được).</summary>
    public static string ComputeSignature(string secret, long timestamp, ReadOnlySpan<byte> rawBody)
    {
        if (string.IsNullOrEmpty(secret)) throw new ArgumentException("Secret webhook không được rỗng.", nameof(secret));

        var prefix = Encoding.UTF8.GetBytes(timestamp.ToString(CultureInfo.InvariantCulture) + ".");
        var payload = new byte[prefix.Length + rawBody.Length];
        prefix.CopyTo(payload, 0);
        rawBody.CopyTo(payload.AsSpan(prefix.Length));

        var hash = HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), payload);
        return SignaturePrefix + Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>Tính chữ ký cho body dạng chuỗi (nguyên văn, không parse rồi serialize lại).</summary>
    public static string ComputeSignature(string secret, long timestamp, string rawBody)
        => ComputeSignature(secret, timestamp, Encoding.UTF8.GetBytes(rawBody ?? throw new ArgumentNullException(nameof(rawBody))));

    /// <summary>Kiểm tra chữ ký và thời gian; trả <c>false</c> nếu không hợp lệ.</summary>
    /// <param name="secret">Secret ký webhook của cửa hàng (<c>whsec_...</c>).</param>
    /// <param name="timestampHeader">Giá trị header <c>X-Webhook-Timestamp</c>.</param>
    /// <param name="signatureHeader">Giá trị header <c>X-Webhook-Signature</c>.</param>
    /// <param name="rawBody">Nguyên văn body nhận được.</param>
    /// <param name="tolerance">Độ lệch thời gian cho phép, mặc định 5 phút.</param>
    /// <param name="now">Thời điểm hiện tại (dùng khi test), mặc định giờ hệ thống.</param>
    public static bool IsValid(string secret, string? timestampHeader, string? signatureHeader, ReadOnlySpan<byte> rawBody,
        TimeSpan? tolerance = null, DateTimeOffset? now = null)
        => Validate(secret, timestampHeader, signatureHeader, rawBody, tolerance, now) == null;

    /// <summary>Kiểm tra chữ ký và thời gian cho body dạng chuỗi.</summary>
    public static bool IsValid(string secret, string? timestampHeader, string? signatureHeader, string rawBody,
        TimeSpan? tolerance = null, DateTimeOffset? now = null)
        => IsValid(secret, timestampHeader, signatureHeader, Encoding.UTF8.GetBytes(rawBody ?? string.Empty), tolerance, now);

    /// <summary>
    /// Kiểm tra chữ ký rồi đọc sự kiện. Ném <see cref="SofiPosWebhookException"/> nếu webhook không hợp lệ.
    /// </summary>
    public static SofiPosWebhookEvent VerifyAndParse(string secret, string? timestampHeader, string? signatureHeader, ReadOnlySpan<byte> rawBody,
        TimeSpan? tolerance = null, DateTimeOffset? now = null)
    {
        var error = Validate(secret, timestampHeader, signatureHeader, rawBody, tolerance, now);
        if (error != null) throw new SofiPosWebhookException(error);
        return Parse(rawBody);
    }

    /// <summary>Kiểm tra chữ ký rồi đọc sự kiện cho body dạng chuỗi.</summary>
    public static SofiPosWebhookEvent VerifyAndParse(string secret, string? timestampHeader, string? signatureHeader, string rawBody,
        TimeSpan? tolerance = null, DateTimeOffset? now = null)
        => VerifyAndParse(secret, timestampHeader, signatureHeader, Encoding.UTF8.GetBytes(rawBody ?? string.Empty), tolerance, now);

    /// <summary>Đọc sự kiện mà không kiểm tra chữ ký (chỉ dùng khi đã kiểm tra ở nơi khác).</summary>
    public static SofiPosWebhookEvent Parse(ReadOnlySpan<byte> rawBody)
    {
        try
        {
            var webhookEvent = JsonSerializer.Deserialize<SofiPosWebhookEvent>(rawBody, SofiPosJson.Options);
            if (webhookEvent == null || string.IsNullOrEmpty(webhookEvent.Event))
            {
                throw new SofiPosWebhookException("Body webhook không có trường event.");
            }

            return webhookEvent;
        }
        catch (JsonException ex)
        {
            throw new SofiPosWebhookException("Body webhook không phải JSON hợp lệ.", ex);
        }
    }

    /// <summary>Đọc sự kiện từ body dạng chuỗi mà không kiểm tra chữ ký.</summary>
    public static SofiPosWebhookEvent Parse(string rawBody)
        => Parse(Encoding.UTF8.GetBytes(rawBody ?? throw new ArgumentNullException(nameof(rawBody))));

    private static string? Validate(string secret, string? timestampHeader, string? signatureHeader, ReadOnlySpan<byte> rawBody,
        TimeSpan? tolerance, DateTimeOffset? now)
    {
        if (string.IsNullOrEmpty(secret)) throw new ArgumentException("Secret webhook không được rỗng.", nameof(secret));

        if (string.IsNullOrWhiteSpace(timestampHeader)
            || !long.TryParse(timestampHeader.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var timestamp))
        {
            return "Thiếu hoặc sai header " + SofiPosWebhookHeaders.Timestamp + ".";
        }

        if (string.IsNullOrWhiteSpace(signatureHeader))
        {
            return "Thiếu header " + SofiPosWebhookHeaders.Signature + ".";
        }

        var allowed = tolerance ?? DefaultTolerance;
        if (allowed > TimeSpan.Zero)
        {
            var current = (now ?? DateTimeOffset.UtcNow).ToUnixTimeSeconds();
            if (Math.Abs(current - timestamp) > allowed.TotalSeconds)
            {
                return "Webhook đã quá thời gian cho phép.";
            }
        }

        var expected = Encoding.ASCII.GetBytes(ComputeSignature(secret, timestamp, rawBody));
        var actual = Encoding.ASCII.GetBytes(signatureHeader.Trim().ToLowerInvariant());
        return CryptographicOperations.FixedTimeEquals(expected, actual) ? null : "Chữ ký webhook không hợp lệ.";
    }
}
