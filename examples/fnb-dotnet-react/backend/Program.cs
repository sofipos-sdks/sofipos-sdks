using Microsoft.AspNetCore.DataProtection;
using SofiPOS.FnB.Demo;
using SofiPOS.FnB.Demo.Services;
using SofiPOS.FnB.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    o.JsonSerializerOptions.Converters.Add(new VietnamDateTimeConverter());
});
builder.Services.AddDataProtection()
    .SetApplicationName("SofiPOS.FnB.Demo")
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, ".local-data", "keys")));
builder.Services.AddSingleton<DemoStore>();
builder.Services.AddSingleton<ConnectionService>();
builder.Services.AddSingleton<ExecutionScope>();
builder.Services.AddTransient<SdkTraceHandler>();
builder.Services.AddHttpClient("sofipos", c => c.Timeout = Timeout.InfiniteTimeSpan)
    .AddHttpMessageHandler<SdkTraceHandler>();
builder.Services.AddSingleton<SdkGateway>();
builder.Services.AddSingleton<SyncService>();
builder.Services.AddHostedService<DemoWorker>();

var app = builder.Build();
app.UseMiddleware<DemoErrors>();
app.MapControllers();
app.MapGet("/api/health", async (DemoStore store, CancellationToken ct) =>
{
    try { await store.PingAsync(ct); return Results.Ok(new { status = "ready", sdk = "SofiPOS.FnB.Sdk", operations = 91 }); }
    catch { return Results.Json(new { status = "mongo-unavailable", message = "Chưa kết nối được MongoDB. Kiểm tra Mongo:ConnectionString." }, statusCode: 503); }
});
app.Run();
