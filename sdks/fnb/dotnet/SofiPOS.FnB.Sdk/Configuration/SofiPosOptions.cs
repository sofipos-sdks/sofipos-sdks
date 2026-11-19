namespace SofiPOS.FnB.Configuration;

/// <summary>
/// Cấu hình kết nối SofiPOS Partner API. Mỗi Partner Key gắn với đúng một cửa hàng,
/// vì vậy mỗi cửa hàng dùng một <see cref="SofiPosOptions"/> (và một <see cref="SofiPosFnBClient"/>) riêng.
/// </summary>
public sealed class SofiPosOptions
{
    /// <summary>Đường dẫn tiền tố của Partner API, SDK tự nối vào sau <see cref="BaseUrl"/>.</summary>
    public const string ApiPathPrefix = "api/partner/v1/";

    /// <summary>Thời gian chờ mặc định cho mỗi lần gọi API.</summary>
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Địa chỉ máy chủ SofiPOS do SofiPOS cung cấp khi bàn giao, ví dụ <c>https://api.example.com</c>.
    /// Có thể kèm sẵn <c>/api/partner/v1</c>, SDK không nối lặp.
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>Partner Key (<c>tp_...</c>) do cửa hàng cấp. Bỏ trống nếu dùng <see cref="ApiKeyProvider"/>.</summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Hàm lấy Partner Key cho từng request (ví dụ đọc từ kho bí mật khi cửa hàng tạo lại key).
    /// Được ưu tiên hơn <see cref="ApiKey"/> khi có giá trị.
    /// </summary>
    public Func<CancellationToken, ValueTask<string>>? ApiKeyProvider { get; set; }

    /// <summary>Thời gian chờ tối đa cho mỗi lần gửi request. Mặc định 30 giây.</summary>
    public TimeSpan Timeout { get; set; } = DefaultTimeout;

    /// <summary>
    /// Số lần thử lại tối đa cho request <c>GET</c> khi gặp lỗi tạm thời (lỗi mạng, 429, 502, 503, 504).
    /// Mặc định 2. Request ghi dữ liệu (<c>POST</c>, <c>PUT</c>) không bao giờ được thử lại tự động.
    /// </summary>
    public int MaxRetries { get; set; } = 2;

    /// <summary>Tạo bản sao độc lập của cấu hình.</summary>
    public SofiPosOptions Clone() => new()
    {
        BaseUrl = BaseUrl,
        ApiKey = ApiKey,
        ApiKeyProvider = ApiKeyProvider,
        Timeout = Timeout,
        MaxRetries = MaxRetries
    };

    /// <summary>Kiểm tra cấu hình hợp lệ, ném <see cref="ArgumentException"/> nếu sai.</summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl)
            || !Uri.TryCreate(BaseUrl.Trim(), UriKind.Absolute, out var uri)
            || (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
        {
            throw new ArgumentException("SofiPosOptions.BaseUrl phải là URL http(s) tuyệt đối.", nameof(BaseUrl));
        }

        if (ApiKeyProvider == null && string.IsNullOrWhiteSpace(ApiKey))
        {
            throw new ArgumentException("Cần cấu hình SofiPosOptions.ApiKey hoặc SofiPosOptions.ApiKeyProvider.", nameof(ApiKey));
        }

        if (Timeout <= TimeSpan.Zero && Timeout != System.Threading.Timeout.InfiniteTimeSpan)
        {
            throw new ArgumentException("SofiPosOptions.Timeout phải lớn hơn 0.", nameof(Timeout));
        }

        if (MaxRetries < 0)
        {
            throw new ArgumentException("SofiPosOptions.MaxRetries không được âm.", nameof(MaxRetries));
        }
    }

    /// <summary>URI gốc của Partner API (luôn kết thúc bằng <c>/api/partner/v1/</c>).</summary>
    internal Uri GetApiBaseUri()
    {
        var baseUrl = BaseUrl.Trim().TrimEnd('/') + "/";
        if (!baseUrl.EndsWith(ApiPathPrefix, StringComparison.OrdinalIgnoreCase))
        {
            baseUrl += ApiPathPrefix;
        }

        return new Uri(baseUrl, UriKind.Absolute);
    }
}
