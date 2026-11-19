using SofiPOS.FnB.Exceptions;
using SofiPOS.FnB.Webhooks;
using SofiPOS.FnB.Demo.Services;

namespace SofiPOS.FnB.Demo;

public sealed class DemoErrors(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested) { }
        catch (Exception error)
        {
            if (error is ConnectionProbeException probe)
            {
                var code = probe.InnerException is SofiPosApiException api ? (int)api.StatusCode : 502;
                context.Response.StatusCode = code;
                await context.Response.WriteAsJsonAsync(new { message = probe.Message, statusCode = code, callId = probe.CallId, trace = probe.Trace });
                return;
            }
            var (status, message, traceId) = error switch
            {
                SofiPosApiException sdk => ((int)sdk.StatusCode, sdk.Message, sdk.TraceId),
                SofiPosWebhookException hook => (401, hook.Message, null),
                ArgumentException arg => (400, arg.Message, null),
                System.Text.Json.JsonException json => (400, json.Message, null),
                TimeoutException => (504, "API POS chưa phản hồi trong thời gian chờ.", null),
                HttpRequestException => (502, "Không kết nối được API POS. Kiểm tra địa chỉ máy chủ.", null),
                OperationCanceledException => (504, "API POS quá thời gian chờ. Không tự gửi lại thao tác ghi.", null),
                InvalidOperationException invalid => (409, invalid.Message, null),
                _ => (500, "Có lỗi khi xử lý. Xem log backend để biết vị trí lỗi.", null)
            };
            if (status == 500) context.RequestServices.GetRequiredService<ILogger<DemoErrors>>().LogError("Demo error: {Type}", error.GetType().FullName);
            context.Response.StatusCode = status;
            await context.Response.WriteAsJsonAsync(new { message, traceId, statusCode = status, callId = error.Data["CallId"] as string,
                connectionRequired = error is ConnectionSessionException });
        }
    }
}
