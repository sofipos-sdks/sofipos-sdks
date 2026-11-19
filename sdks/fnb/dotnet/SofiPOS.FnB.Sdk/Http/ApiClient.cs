using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using SofiPOS.FnB.Authentication;
using SofiPOS.FnB.Configuration;
using SofiPOS.FnB.Exceptions;

namespace SofiPOS.FnB.Http;

/// <summary>
/// Lớp HTTP dùng chung cho mọi service: dựng URL, gắn xác thực, serialize JSON, timeout, thử lại GET an toàn và đọc lỗi.
/// An toàn khi gọi đồng thời; không giữ trạng thái thay đổi theo request.
/// </summary>
internal sealed class ApiClient
{
    /// <summary>Header mã truy vết request của SofiPOS.</summary>
    public const string TraceIdHeader = "trace-id";

    private const int MaxBodyInException = 4000;
    private static readonly MediaTypeHeaderValue JsonContentType = new("application/json") { CharSet = "utf-8" };

    private readonly HttpClient _httpClient;
    private readonly SofiPosOptions _options;
    private readonly PartnerAuthenticationHandler _authentication;
    private readonly Uri _baseUri;

    public ApiClient(HttpClient httpClient, SofiPosOptions options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _options.Validate();
        _authentication = new PartnerAuthenticationHandler(_options);
        _baseUri = _options.GetApiBaseUri();
    }

    /// <summary>Hàm chờ giữa các lần thử lại; thay được trong unit test.</summary>
    internal Func<TimeSpan, CancellationToken, Task> Delay { get; set; } = Task.Delay;

    public Task<T> GetAsync<T>(string path, QueryString? query, CancellationToken cancellationToken)
        => SendAsync<T>(HttpMethod.Get, path, query, null, hasBody: false, cancellationToken);

    public Task<T> PostAsync<T>(string path, object? body, CancellationToken cancellationToken, QueryString? query = null)
        => SendAsync<T>(HttpMethod.Post, path, query, body, hasBody: true, cancellationToken);

    public Task<T> PutAsync<T>(string path, object? body, CancellationToken cancellationToken, QueryString? query = null)
        => SendAsync<T>(HttpMethod.Put, path, query, body, hasBody: true, cancellationToken);

    /// <summary>Gửi request không cần đọc dữ liệu trả về (ví dụ xóa); thành công khi mã HTTP 2xx.</summary>
    public async Task SendWithoutResultAsync(HttpMethod method, string path, object? body, CancellationToken cancellationToken)
    {
        using var response = await SendCoreAsync(method, path, null, body, hasBody: body != null, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);
    }

    private async Task<T> SendAsync<T>(HttpMethod method, string path, QueryString? query, object? body, bool hasBody, CancellationToken cancellationToken)
    {
        using var response = await SendCoreAsync(method, path, query, body, hasBody, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, cancellationToken).ConfigureAwait(false);

        var content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(content))
        {
            throw new SofiPosApiException(response.StatusCode, "API trả về phản hồi rỗng trong khi cần dữ liệu.", traceId: GetTraceId(response));
        }

        try
        {
            var result = JsonSerializer.Deserialize<T>(content, SofiPosJson.Options);
            return result ?? throw new JsonException("Phản hồi JSON là null.");
        }
        catch (JsonException ex)
        {
            throw new SofiPosApiException(response.StatusCode, "Không đọc được dữ liệu trả về của API: " + ex.Message,
                traceId: GetTraceId(response), responseBody: Truncate(content), innerException: ex);
        }
    }

    private async Task<HttpResponseMessage> SendCoreAsync(HttpMethod method, string path, QueryString? query, object? body, bool hasBody, CancellationToken cancellationToken)
    {
        var uri = new Uri(_baseUri, path.TrimStart('/') + query);
        var payload = hasBody && body != null ? JsonSerializer.Serialize(body, body.GetType(), SofiPosJson.Options) : null;
        // Chỉ GET (đọc dữ liệu) được thử lại; POST/PUT có thể tạo/ghi dữ liệu nên không tự gửi lại.
        var maxAttempts = method == HttpMethod.Get ? _options.MaxRetries + 1 : 1;

        for (var attempt = 1; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var request = new HttpRequestMessage(method, uri);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            if (payload != null)
            {
                request.Content = new StringContent(payload, Encoding.UTF8);
                request.Content.Headers.ContentType = JsonContentType;
            }

            await _authentication.ApplyAsync(request, cancellationToken).ConfigureAwait(false);

            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            if (_options.Timeout != Timeout.InfiniteTimeSpan) timeoutCts.CancelAfter(_options.Timeout);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, timeoutCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
            {
                if (attempt < maxAttempts)
                {
                    await Delay(GetBackoff(attempt, null), cancellationToken).ConfigureAwait(false);
                    continue;
                }

                throw new TimeoutException($"Request {method} {path} vượt quá thời gian chờ {_options.Timeout.TotalSeconds:0.#} giây.");
            }
            catch (HttpRequestException) when (attempt < maxAttempts)
            {
                await Delay(GetBackoff(attempt, null), cancellationToken).ConfigureAwait(false);
                continue;
            }

            if (attempt < maxAttempts && IsRetryable(response.StatusCode))
            {
                var wait = GetBackoff(attempt, response.Headers.RetryAfter);
                response.Dispose();
                await Delay(wait, cancellationToken).ConfigureAwait(false);
                continue;
            }

            return response;
        }
    }

    private static bool IsRetryable(HttpStatusCode statusCode)
        => (int)statusCode == 429 || statusCode is HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout;

    private static TimeSpan GetBackoff(int attempt, RetryConditionHeaderValue? retryAfter)
    {
        if (retryAfter?.Delta is { } delta && delta > TimeSpan.Zero)
        {
            return delta < TimeSpan.FromSeconds(30) ? delta : TimeSpan.FromSeconds(30);
        }

        if (retryAfter?.Date is { } date)
        {
            var wait = date - DateTimeOffset.UtcNow;
            if (wait > TimeSpan.Zero) return wait < TimeSpan.FromSeconds(30) ? wait : TimeSpan.FromSeconds(30);
        }

        // 0,5s; 1s; 2s... kèm dao động nhỏ để các client không gọi lại cùng lúc.
        var baseMs = 500 * Math.Pow(2, attempt - 1);
        return TimeSpan.FromMilliseconds(Math.Min(baseMs, 8000) + Random.Shared.Next(0, 250));
    }

    private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        if (response.IsSuccessStatusCode) return;

        var body = response.Content == null ? null : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        throw CreateException(response.StatusCode, body, GetTraceId(response));
    }

    internal static SofiPosApiException CreateException(HttpStatusCode statusCode, string? body, string? traceId)
    {
        string? message = null;
        int? errorCode = null;
        var errors = new List<SofiPosValidationError>();

        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.String)
                {
                    message = root.GetString();
                }
                else if (root.ValueKind == JsonValueKind.Object)
                {
                    message = GetString(root, "message") ?? GetString(root, "detail") ?? GetString(root, "title");
                    if (TryGetProperty(root, "codeError", out var code) && code.ValueKind == JsonValueKind.Number
                        && code.TryGetInt32(out var codeValue) && codeValue != 0)
                    {
                        errorCode = codeValue;
                    }

                    ReadErrors(root, errors);
                }
            }
            catch (JsonException)
            {
                // Phản hồi không phải JSON (ví dụ trang lỗi của proxy): giữ nguyên trong ResponseBody.
            }
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            message = errors.FirstOrDefault()?.Message ?? DefaultMessage(statusCode);
        }

        return new SofiPosApiException(statusCode, message!, errorCode, traceId, errors, Truncate(body));
    }

    private static void ReadErrors(JsonElement root, List<SofiPosValidationError> errors)
    {
        // Định dạng của SofiPOS: "details": [{ "field", "message" }]
        if (TryGetProperty(root, "details", out var details) && details.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in details.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object) continue;
                var text = GetString(item, "message");
                if (!string.IsNullOrWhiteSpace(text)) errors.Add(new SofiPosValidationError(GetString(item, "field"), text!));
            }
        }

        // Định dạng ProblemDetails của ASP.NET Core: "errors": { "field": ["message"] }
        if (TryGetProperty(root, "errors", out var problemErrors) && problemErrors.ValueKind == JsonValueKind.Object)
        {
            foreach (var field in problemErrors.EnumerateObject())
            {
                if (field.Value.ValueKind != JsonValueKind.Array) continue;
                foreach (var item in field.Value.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String) errors.Add(new SofiPosValidationError(field.Name, item.GetString()!));
                }
            }
        }
    }

    private static bool TryGetProperty(JsonElement element, string name, out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static string? GetString(JsonElement element, string name)
        => TryGetProperty(element, name, out var value) && value.ValueKind == JsonValueKind.String ? value.GetString() : null;

    private static string DefaultMessage(HttpStatusCode statusCode) => (int)statusCode switch
    {
        401 => "Partner Key không hợp lệ, đã bị thay hoặc đối tác đã ngừng hoạt động.",
        403 => "Không có quyền thực hiện thao tác này.",
        404 => "Dữ liệu không tồn tại hoặc thuộc cửa hàng khác.",
        409 => "Dữ liệu đã được thay đổi đồng thời, vui lòng đọc lại rồi thử lại.",
        429 => "Gọi API quá nhiều, vui lòng thử lại sau.",
        >= 500 => "Hệ thống SofiPOS đang lỗi hoặc quá tải, vui lòng thử lại sau.",
        _ => $"SofiPOS API trả về mã HTTP {(int)statusCode}."
    };

    private static string? GetTraceId(HttpResponseMessage response)
        => response.Headers.TryGetValues(TraceIdHeader, out var values) ? values.FirstOrDefault() : null;

    private static string? Truncate(string? body)
        => body == null || body.Length <= MaxBodyInException ? body : body[..MaxBodyInException];
}
