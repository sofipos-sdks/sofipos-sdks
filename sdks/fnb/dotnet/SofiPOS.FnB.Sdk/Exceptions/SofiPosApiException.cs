using System.Net;

namespace SofiPOS.FnB.Exceptions;

/// <summary>
/// Lỗi do SofiPOS Partner API trả về (mã HTTP ngoài 2xx) hoặc phản hồi không đọc được.
/// Không chứa Partner Key hay header xác thực.
/// </summary>
public class SofiPosApiException : Exception
{
    /// <summary>Tạo exception.</summary>
    public SofiPosApiException(
        HttpStatusCode statusCode,
        string message,
        int? errorCode = null,
        string? traceId = null,
        IReadOnlyList<SofiPosValidationError>? errors = null,
        string? responseBody = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        TraceId = traceId;
        Errors = errors ?? Array.Empty<SofiPosValidationError>();
        ResponseBody = responseBody;
    }

    /// <summary>Mã HTTP.</summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>Mã lỗi nghiệp vụ (<c>codeError</c>) nếu API trả về khác 0.</summary>
    public int? ErrorCode { get; }

    /// <summary>
    /// Mã truy vết của request (header <c>trace-id</c>), gửi kèm khi liên hệ hỗ trợ SofiPOS.
    /// </summary>
    public string? TraceId { get; }

    /// <summary>Lỗi kiểm tra dữ liệu theo từng trường (<c>details</c>); rỗng với lỗi nghiệp vụ.</summary>
    public IReadOnlyList<SofiPosValidationError> Errors { get; }

    /// <summary>Nội dung phản hồi gốc (tối đa 4.000 ký tự), phục vụ ghi log và điều tra.</summary>
    public string? ResponseBody { get; }

    /// <summary><c>401</c>: Partner Key sai, đã bị thay hoặc đối tác bị ngừng hoạt động.</summary>
    public bool IsUnauthorized => StatusCode == HttpStatusCode.Unauthorized;

    /// <summary><c>404</c>: dữ liệu không tồn tại hoặc thuộc cửa hàng khác.</summary>
    public bool IsNotFound => StatusCode == HttpStatusCode.NotFound;

    /// <summary><c>409</c>: xung đột dữ liệu khi ghi đồng thời, nên đọc lại rồi thử lại.</summary>
    public bool IsConflict => StatusCode == HttpStatusCode.Conflict;

    /// <summary><c>400</c>/<c>422</c>: dữ liệu không hợp lệ hoặc vi phạm nghiệp vụ.</summary>
    public bool IsValidationError => StatusCode is HttpStatusCode.BadRequest or HttpStatusCode.UnprocessableEntity;

    /// <summary>Lỗi tạm thời (<c>429</c>, <c>5xx</c>), có thể thử lại sau.</summary>
    public bool IsTransient => (int)StatusCode == 429 || (int)StatusCode >= 500;
}

/// <summary>Lỗi kiểm tra dữ liệu của một trường.</summary>
/// <param name="Field">Tên trường do API trả về.</param>
/// <param name="Message">Thông báo lỗi.</param>
public sealed record SofiPosValidationError(string? Field, string Message);
