using System.Net;

namespace SofiPOS.FnB.Sdk.Tests;

public class ResilienceTests
{
    [Fact]
    public async Task Get_is_retried_on_429_and_honours_retry_after()
    {
        var delays = new List<TimeSpan>();
        var handler = new MockHttpHandler()
            .Respond((HttpStatusCode)429, null, r => r.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(3)))
            .RespondJson("[]");
        var client = TestClient.Create(handler);
        client.Api.Delay = (d, _) => { delays.Add(d); return Task.CompletedTask; };

        var branches = await client.Lookups.GetBranchesAsync();

        Assert.Empty(branches);
        Assert.Equal(2, handler.Requests.Count);
        Assert.Equal(TimeSpan.FromSeconds(3), Assert.Single(delays));
    }

    [Fact]
    public async Task Get_is_retried_on_network_error_then_gives_up()
    {
        var handler = new MockHttpHandler()
            .Throw(new HttpRequestException("reset"))
            .Throw(new HttpRequestException("reset"))
            .Throw(new HttpRequestException("reset"));
        var client = TestClient.Create(handler, o => o.MaxRetries = 2);

        await Assert.ThrowsAsync<HttpRequestException>(() => client.Lookups.GetBranchesAsync());

        Assert.Equal(3, handler.Requests.Count);
    }

    [Fact]
    public async Task Get_is_not_retried_on_500()
    {
        var handler = new MockHttpHandler().Respond(HttpStatusCode.InternalServerError).RespondJson("[]");
        var client = TestClient.Create(handler);

        await Assert.ThrowsAsync<SofiPosApiException>(() => client.Lookups.GetBranchesAsync());

        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task Create_order_is_never_retried()
    {
        var handler = new MockHttpHandler().Respond(HttpStatusCode.ServiceUnavailable).RespondJson(RequestTests.OrderJson);
        var client = TestClient.Create(handler, o => o.MaxRetries = 5);

        var ex = await Assert.ThrowsAsync<SofiPosApiException>(() => client.FnbOrders.CreateAsync(new FnbOrderCreateRequest
        {
            RequestId = Guid.NewGuid(),
            Items = { new FnbOrderItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 } }
        }));

        Assert.Equal(HttpStatusCode.ServiceUnavailable, ex.StatusCode);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task Writes_are_not_retried_on_network_error_or_429()
    {
        var handler = new MockHttpHandler()
            .Throw(new HttpRequestException("reset"))
            .Respond((HttpStatusCode)429)
            .RespondJson(RequestTests.OrderJson);
        var client = TestClient.Create(handler);

        await Assert.ThrowsAsync<HttpRequestException>(() => client.FnbPayments.PayAsync(Guid.NewGuid(), new FnbPaymentRequest { PaymentMethod = PaymentMethod.Cash }));
        await Assert.ThrowsAsync<SofiPosApiException>(() => client.FnbOrders.CancelAsync(Guid.NewGuid(), "Khách đổi ý"));

        Assert.Equal(2, handler.Requests.Count);
    }

    [Fact]
    public async Task Cancellation_token_is_honoured()
    {
        var handler = new MockHttpHandler().Respond(async (_, ct) =>
        {
            await Task.Delay(Timeout.Infinite, ct);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        var client = TestClient.Create(handler);
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.Lookups.GetBranchesAsync(cts.Token));
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task Already_cancelled_token_sends_nothing()
    {
        var handler = new MockHttpHandler();
        var client = TestClient.Create(handler);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => client.Lookups.GetBranchesAsync(new CancellationToken(true)));
        Assert.Empty(handler.Requests);
    }

    [Fact]
    public async Task Timeout_throws_timeout_exception()
    {
        var handler = new MockHttpHandler().Respond(async (_, ct) =>
        {
            await Task.Delay(Timeout.Infinite, ct);
            return new HttpResponseMessage(HttpStatusCode.OK);
        });
        var client = TestClient.Create(handler, o => { o.Timeout = TimeSpan.FromMilliseconds(100); o.MaxRetries = 0; });

        var ex = await Assert.ThrowsAsync<TimeoutException>(() => client.FnbOrders.CreateAsync(new FnbOrderCreateRequest
        {
            RequestId = Guid.NewGuid(),
            Items = { new FnbOrderItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 } }
        }));

        Assert.DoesNotContain(TestClient.ApiKey, ex.Message);
        Assert.Single(handler.Requests);
    }

    [Fact]
    public async Task Concurrent_calls_with_different_keys_do_not_mix_headers()
    {
        var handler = new MockHttpHandler();
        for (var i = 0; i < 20; i++) handler.RespondJson("[]");
        var http = new HttpClient(handler);
        var a = new SofiPosFnBClient(http, new SofiPOS.FnB.Configuration.SofiPosOptions { BaseUrl = TestClient.BaseUrl, ApiKey = "tp_store_a" });
        var b = new SofiPosFnBClient(http, new SofiPOS.FnB.Configuration.SofiPosOptions { BaseUrl = TestClient.BaseUrl, ApiKey = "tp_store_b" });

        var tasks = Enumerable.Range(0, 20).Select(i => (i % 2 == 0 ? a : b).Lookups.GetBranchesAsync());
        await Task.WhenAll(tasks);

        Assert.Equal(10, handler.Requests.Count(r => r.Headers["X-Partner-Key"] == "tp_store_a"));
        Assert.Equal(10, handler.Requests.Count(r => r.Headers["X-Partner-Key"] == "tp_store_b"));
    }
}
