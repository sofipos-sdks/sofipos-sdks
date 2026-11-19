using SofiPOS.FnB.Demo;
using SofiPOS.FnB.Demo.Services;
using SofiPOS.FnB.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    o.JsonSerializerOptions.Converters.Add(new VietnamDateTimeConverter());
});
builder.Services.AddSingleton<DemoStore>();
builder.Services.AddSingleton<WorkspaceSession>();
builder.Services.AddSingleton<ConnectionService>();
builder.Services.AddSingleton<ExecutionScope>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<SdkTraceHandler>();
builder.Services.AddHttpClient("sofipos", c => c.Timeout = Timeout.InfiniteTimeSpan).AddHttpMessageHandler<SdkTraceHandler>();
builder.Services.AddSingleton<SdkGateway>();

var app = builder.Build();
app.UseMiddleware<DemoErrors>();
app.Use(async (context, next) =>
{
    var session = context.RequestServices.GetRequiredService<WorkspaceSession>();
    await session.RunAsync(async () =>
    {
        var path = context.Request.Path;
        var expected = context.Request.Headers["X-Workspace-Connection"].ToString();
        var connections = context.RequestServices.GetRequiredService<ConnectionService>();
        if (!string.IsNullOrEmpty(expected) && path.StartsWithSegments("/api") && path != "/api/health" &&
            !(path == "/api/connection" && HttpMethods.IsGet(context.Request.Method)) && path != "/api/connection/check")
        {
            if ((await connections.GetAsync(context.RequestAborted))?.ConnectionId != expected)
                throw new ConnectionSessionException("Phiên kết nối đã thay đổi hoặc backend đã khởi động lại. Nhập lại kết nối trước khi tiếp tục.");
        }
        if (path.StartsWithSegments("/api") && !path.StartsWithSegments("/api/connection") && path != "/api/health")
            await connections.RequiredAsync(context.RequestAborted);
        await next(context);
    }, context.RequestAborted);
});
app.MapControllers();
app.MapGet("/api/health", () => Results.Ok(new { status = "ready", sdk = "SofiPOS.FnB.Sdk", operations = 92, historyLimit = DemoStore.HistoryLimit }));
app.Run();
