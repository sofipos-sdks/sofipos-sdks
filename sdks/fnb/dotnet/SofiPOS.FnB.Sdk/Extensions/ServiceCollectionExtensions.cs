using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using SofiPOS.FnB.Configuration;

namespace SofiPOS.FnB.Extensions;

/// <summary>Đăng ký SofiPOS FnB SDK vào ASP.NET Core Dependency Injection.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Tên HttpClient của SDK trong <see cref="IHttpClientFactory"/>.</summary>
    public const string HttpClientName = "SofiPOS.FnB";

    /// <summary>
    /// Đăng ký <see cref="SofiPosFnBClient"/> (typed client, vòng đời transient, HttpClient do <see cref="IHttpClientFactory"/> quản lý)
    /// và <see cref="ISofiPosFnBClientFactory"/> để tạo client cho nhiều cửa hàng.
    /// </summary>
    /// <param name="services">Service collection.</param>
    /// <param name="configure">
    /// Cấu hình mặc định. Ứng dụng nhiều cửa hàng có thể chỉ đặt <see cref="SofiPosOptions.BaseUrl"/> rồi tạo client theo
    /// Partner Key bằng <see cref="ISofiPosFnBClientFactory"/>.
    /// </param>
    /// <returns><see cref="IHttpClientBuilder"/> để cấu hình thêm handler (proxy, logging...).</returns>
    public static IHttpClientBuilder AddSofiPosFnB(this IServiceCollection services, Action<SofiPosOptions> configure)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configure);

        services.AddOptions<SofiPosOptions>().Configure(configure);
        services.TryAddSingleton<ISofiPosFnBClientFactory, SofiPosFnBClientFactory>();

        return services.AddHttpClient(HttpClientName)
            // Timeout theo từng request do SDK quản lý (SofiPosOptions.Timeout).
            .ConfigureHttpClient(client => client.Timeout = Timeout.InfiniteTimeSpan)
            .ConfigurePrimaryHttpMessageHandler(SofiPosFnBClient.CreateDefaultHandler)
            .AddTypedClient((httpClient, provider) =>
                new SofiPosFnBClient(httpClient, provider.GetRequiredService<IOptions<SofiPosOptions>>().Value));
    }
}

/// <summary>Tạo <see cref="SofiPosFnBClient"/> cho từng cửa hàng (mỗi cửa hàng một Partner Key).</summary>
public interface ISofiPosFnBClientFactory
{
    /// <summary>Tạo client với cấu hình riêng.</summary>
    SofiPosFnBClient CreateClient(SofiPosOptions options);

    /// <summary>Tạo client dùng cấu hình mặc định đã đăng ký, thay Partner Key bằng <paramref name="apiKey"/>.</summary>
    SofiPosFnBClient CreateClient(string apiKey);
}

internal sealed class SofiPosFnBClientFactory : ISofiPosFnBClientFactory
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptionsMonitor<SofiPosOptions> _options;

    public SofiPosFnBClientFactory(IHttpClientFactory httpClientFactory, IOptionsMonitor<SofiPosOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        _options = options;
    }

    public SofiPosFnBClient CreateClient(SofiPosOptions options)
        => new(_httpClientFactory.CreateClient(ServiceCollectionExtensions.HttpClientName), options);

    public SofiPosFnBClient CreateClient(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey)) throw new ArgumentException("Partner Key không được rỗng.", nameof(apiKey));
        var options = _options.CurrentValue.Clone();
        options.ApiKey = apiKey;
        options.ApiKeyProvider = null;
        return CreateClient(options);
    }
}
