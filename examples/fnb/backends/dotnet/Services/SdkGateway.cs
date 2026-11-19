using SofiPOS.FnB;
using SofiPOS.FnB.Configuration;

namespace SofiPOS.FnB.Demo.Services;

public sealed record SdkResult<T>(T Data, string CallId);

public sealed class SdkGateway(ConnectionService connections, IHttpClientFactory http, ExecutionScope execution,
    WorkspaceSession session, IHttpContextAccessor accessor)
{
    // Giữ chữ ký bridge để các controller tiếp tục ánh xạ SDK tường minh.
    public Task<SdkResult<T>> CallAsync<T>(string operation, Func<SofiPosFnBClient, CancellationToken, Task<T>> action,
        string? id = null, string? context = null, string source = "manual", Guid? branchId = null, CancellationToken ct = default)
        => session.RunAsync(async () =>
        {
            var connection = await connections.RequiredAsync(ct);
            var previous = execution.Current;
            var info = new ExecutionInfo { ConnectionId = connection.ConnectionId, Operation = operation,
                Source = string.Equals(accessor.HttpContext?.Request.Headers["X-SDK-Source"].ToString(), "lookup", StringComparison.Ordinal) ? "lookup" : source, ApiKey = connection.ApiKey };
            execution.Current = info;
            try
            {
                using var client = new SofiPosFnBClient(http.CreateClient("sofipos"), new SofiPosOptions { BaseUrl = connection.BaseUrl, ApiKey = connection.ApiKey });
                return new SdkResult<T>(await action(client, ct), info.Id);
            }
            catch (Exception error) { error.Data["CallId"] = info.Id; throw; }
            finally { execution.Current = previous; }
        }, ct);

    public Task<SdkResult<object?>> VoidAsync(string operation, Func<SofiPosFnBClient, CancellationToken, Task> action,
        string? id = null, CancellationToken ct = default)
        => CallAsync<object?>(operation, async (client, token) => { await action(client, token); return null; }, id:id, ct:ct);
}
