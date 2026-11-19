using MongoDB.Driver;
using SofiPOS.FnB.Exceptions;
using SofiPOS.FnB.Webhooks;

namespace SofiPOS.FnB.Demo;

public sealed class DemoErrors(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested) { }
        catch (Exception error)
        {
            var (status, message, traceId) = error switch
            {
                SofiPosApiException sdk => ((int)sdk.StatusCode, sdk.Message, sdk.TraceId),
                SofiPosWebhookException hook => (401, hook.Message, null),
                ArgumentException arg => (400, arg.Message, null),
                System.Text.Json.JsonException json => (400, json.Message, null),
                MongoException => (503, "Không kết nối được MongoDB. Kiểm tra cấu hình Mongo:ConnectionString và trạng thái MongoDB.", null),
                TimeoutException => (503, "MongoDB hoặc API chưa phản hồi trong thời gian chờ.", null),
                HttpRequestException => (502, "Không kết nối được API POS. Kiểm tra địa chỉ máy chủ.", null),
                OperationCanceledException => (504, "API POS quá thời gian chờ. Không tự gửi lại thao tác ghi.", null),
                InvalidOperationException invalid => (409, invalid.Message, null),
                _ => (500, "Có lỗi khi xử lý. Xem log backend để biết vị trí lỗi.", null)
            };
            if (status == 500) context.RequestServices.GetRequiredService<ILogger<DemoErrors>>().LogError("Demo error: {Type}", error.GetType().FullName);
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new { message, traceId, statusCode = status });
        }
    }
}
