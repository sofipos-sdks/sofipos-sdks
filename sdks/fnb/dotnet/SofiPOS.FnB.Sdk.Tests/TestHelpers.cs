using System.Net;
using System.Text;
using SofiPOS.FnB.Configuration;

namespace SofiPOS.FnB.Sdk.Tests;

/// <summary>HttpMessageHandler giả: ghi lại request và trả phản hồi theo hàng đợi.</summary>
internal sealed class MockHttpHandler : HttpMessageHandler
{
    private readonly Queue<Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>> _responses = new();

    public List<CapturedRequest> Requests { get; } = new();

    public MockHttpHandler Respond(HttpStatusCode status, string? body = null, Action<HttpResponseMessage>? configure = null)
    {
        _responses.Enqueue((_, _) =>
        {
            var response = new HttpResponseMessage(status);
            if (body != null) response.Content = new StringContent(body, Encoding.UTF8, "application/json");
            configure?.Invoke(response);
            return Task.FromResult(response);
        });
        return this;
    }

    public MockHttpHandler RespondJson(string body) => Respond(HttpStatusCode.OK, body);

    public MockHttpHandler Respond(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder)
    {
        _responses.Enqueue(responder);
        return this;
    }

    public MockHttpHandler Throw(Exception exception)
    {
        _responses.Enqueue((_, _) => Task.FromException<HttpResponseMessage>(exception));
        return this;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        Requests.Add(new CapturedRequest(request.Method, request.RequestUri!, body,
            request.Headers.ToDictionary(h => h.Key, h => string.Join(",", h.Value), StringComparer.OrdinalIgnoreCase),
            request.Content?.Headers.ContentType?.ToString()));

        if (_responses.Count == 0) throw new InvalidOperationException("Không còn phản hồi giả cho request " + request.RequestUri);
        return await _responses.Dequeue()(request, cancellationToken);
    }
}

internal sealed record CapturedRequest(HttpMethod Method, Uri Uri, string? Body, Dictionary<string, string> Headers, string? ContentType);

internal static class TestClient
{
    public const string BaseUrl = "https://api.test.local";
    public const string ApiKey = "tp_0123456789abcdef0123456789abcdef_SECRETPART0123456789abcdefABCDEF012345";

    public static SofiPosFnBClient Create(MockHttpHandler handler, Action<SofiPosOptions>? configure = null)
    {
        var options = new SofiPosOptions { BaseUrl = BaseUrl, ApiKey = ApiKey };
        configure?.Invoke(options);
        var client = new SofiPosFnBClient(new HttpClient(handler), options);
        client.Api.Delay = (_, _) => Task.CompletedTask;
        return client;
    }
}
