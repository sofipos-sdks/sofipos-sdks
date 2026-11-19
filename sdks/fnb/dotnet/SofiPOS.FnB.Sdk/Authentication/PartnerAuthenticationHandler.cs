using SofiPOS.FnB.Configuration;

namespace SofiPOS.FnB.Authentication;

/// <summary>
/// Gắn header <c>X-Partner-Key</c> vào từng request. Key được đọc lại mỗi lần gửi
/// (từ <see cref="SofiPosOptions.ApiKeyProvider"/> hoặc <see cref="SofiPosOptions.ApiKey"/>), không lưu ở biến static.
/// </summary>
internal sealed class PartnerAuthenticationHandler
{
    /// <summary>Tên header xác thực của Partner API.</summary>
    public const string HeaderName = "X-Partner-Key";

    private readonly SofiPosOptions _options;

    public PartnerAuthenticationHandler(SofiPosOptions options)
    {
        _options = options;
    }

    public async ValueTask ApplyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var key = _options.ApiKeyProvider != null
            ? await _options.ApiKeyProvider(cancellationToken).ConfigureAwait(false)
            : _options.ApiKey;

        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("Chưa cấu hình Partner Key (SofiPosOptions.ApiKey hoặc ApiKeyProvider).");
        }

        request.Headers.Remove(HeaderName);
        request.Headers.TryAddWithoutValidation(HeaderName, key.Trim());
    }
}
