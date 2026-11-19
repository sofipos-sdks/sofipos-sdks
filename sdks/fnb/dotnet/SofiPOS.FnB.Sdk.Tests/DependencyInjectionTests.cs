using Microsoft.Extensions.DependencyInjection;
using SofiPOS.FnB.Extensions;

namespace SofiPOS.FnB.Sdk.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public async Task AddSofiPosFnB_registers_typed_client()
    {
        var handler = new MockHttpHandler().RespondJson("[]");
        var services = new ServiceCollection();
        services.AddSofiPosFnB(o =>
        {
            o.BaseUrl = TestClient.BaseUrl;
            o.ApiKey = "tp_from_config";
        }).ConfigurePrimaryHttpMessageHandler(() => handler);

        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<SofiPosFnBClient>();
        await client.Lookups.GetBranchesAsync();

        Assert.Equal("tp_from_config", handler.Requests[0].Headers["X-Partner-Key"]);
        Assert.Equal("https://api.test.local/api/partner/v1/lookups/branches", handler.Requests[0].Uri.ToString());
    }

    [Fact]
    public async Task Factory_creates_client_per_store_key()
    {
        var handler = new MockHttpHandler().RespondJson("[]").RespondJson("[]");
        var services = new ServiceCollection();
        services.AddSofiPosFnB(o => o.BaseUrl = TestClient.BaseUrl).ConfigurePrimaryHttpMessageHandler(() => handler);

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ISofiPosFnBClientFactory>();
        await factory.CreateClient("tp_store_1").Lookups.GetBranchesAsync();
        await factory.CreateClient("tp_store_2").Lookups.GetBranchesAsync();

        Assert.Equal("tp_store_1", handler.Requests[0].Headers["X-Partner-Key"]);
        Assert.Equal("tp_store_2", handler.Requests[1].Headers["X-Partner-Key"]);
    }

    [Fact]
    public void Factory_does_not_mutate_registered_options()
    {
        var services = new ServiceCollection();
        services.AddSofiPosFnB(o => { o.BaseUrl = TestClient.BaseUrl; o.ApiKey = "tp_default"; });

        using var provider = services.BuildServiceProvider();
        var factory = provider.GetRequiredService<ISofiPosFnBClientFactory>();
        var storeClient = factory.CreateClient("tp_other");
        var defaultClient = provider.GetRequiredService<SofiPosFnBClient>();

        Assert.Equal("tp_other", storeClient.Options.ApiKey);
        Assert.Equal("tp_default", defaultClient.Options.ApiKey);
    }

    [Fact]
    public async Task Client_without_di_works_with_custom_handler()
    {
        var handler = new MockHttpHandler().RespondJson("[]");
        using var client = SofiPosFnBClient.Create(new SofiPOS.FnB.Configuration.SofiPosOptions { BaseUrl = TestClient.BaseUrl, ApiKey = "tp_plain" }, handler);

        await client.Lookups.GetBranchesAsync();

        Assert.Equal("tp_plain", handler.Requests[0].Headers["X-Partner-Key"]);
    }
}
