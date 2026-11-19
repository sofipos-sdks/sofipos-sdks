using System.Net;
using SofiPOS.FnB.Configuration;
using SofiPOS.FnB.Http;
using SofiPOS.FnB.Services;

namespace SofiPOS.FnB;

/// <summary>
/// Client chính của SofiPOS FnB Partner API. Mỗi instance gắn với một Partner Key (một cửa hàng),
/// an toàn khi dùng đồng thời từ nhiều thread.
/// </summary>
/// <example>
/// <code>
/// using var client = new SofiPosFnBClient(new SofiPosOptions
/// {
///     BaseUrl = "https://api.example.com",
///     ApiKey = "YOUR_PARTNER_API_KEY"
/// });
/// var products = await client.Products.ListAsync();
/// </code>
/// </example>
public sealed class SofiPosFnBClient : IDisposable
{
    private static readonly Lazy<HttpClient> SharedHttpClient = new(CreateDefaultHttpClient, LazyThreadSafetyMode.ExecutionAndPublication);
    private readonly HttpClient? _ownedHttpClient;

    /// <summary>
    /// Tạo client không cần Dependency Injection. Các client tạo bằng constructor này dùng chung một
    /// <see cref="HttpClient"/> nội bộ (connection pool được làm mới định kỳ), không gây cạn socket.
    /// </summary>
    /// <param name="options">Cấu hình kết nối; SDK giữ bản sao, sửa đối tượng sau khi tạo client không có tác dụng.</param>
    public SofiPosFnBClient(SofiPosOptions options)
        : this(SharedHttpClient.Value, options)
    {
    }

    /// <summary>
    /// Tạo client dùng <see cref="HttpClient"/> do ứng dụng quản lý (ví dụ từ <c>IHttpClientFactory</c>).
    /// Client không dispose <paramref name="httpClient"/>.
    /// </summary>
    /// <param name="httpClient">HttpClient dùng để gửi request.</param>
    /// <param name="options">Cấu hình kết nối; SDK giữ bản sao.</param>
    public SofiPosFnBClient(HttpClient httpClient, SofiPosOptions options)
        : this(httpClient, options, ownsHttpClient: false)
    {
    }

    private SofiPosFnBClient(HttpClient httpClient, SofiPosOptions options, bool ownsHttpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentNullException.ThrowIfNull(options);

        Options = options.Clone();
        var api = new ApiClient(httpClient, Options);
        Api = api;
        if (ownsHttpClient) _ownedHttpClient = httpClient;

        Lookups = new LookupsService(api);
        Products = new ProductsService(api);
        QrOrderCodes = new QrOrderCodesService(api);
        Customers = new CustomersService(api);
        PricingPolicies = new PricingPoliciesService(api);
        SaleChannels = new SaleChannelsService(api);
        Combos = new CombosService(api);
        FlashSales = new FlashSalesService(api);
        Vouchers = new VouchersService(api);
        OrderAdjustmentPrograms = new OrderAdjustmentProgramsService(api);
        Fnb = new FnbService(api);
        FnbOrders = new FnbOrdersService(api);
        FnbPayments = new FnbPaymentsService(api);
    }

    internal ApiClient Api { get; }

    /// <summary>Cấu hình đang dùng (bản sao, chỉ đọc theo quy ước).</summary>
    public SofiPosOptions Options { get; }

    /// <summary>Chi nhánh, hạng thành viên.</summary>
    public LookupsService Lookups { get; }

    /// <summary>Sản phẩm.</summary>
    public ProductsService Products { get; }

    /// <summary>Mã QR đặt món.</summary>
    public QrOrderCodesService QrOrderCodes { get; }

    /// <summary>Khách hàng.</summary>
    public CustomersService Customers { get; }

    /// <summary>Chính sách giá sản phẩm.</summary>
    public PricingPoliciesService PricingPolicies { get; }

    /// <summary>Kênh bán.</summary>
    public SaleChannelsService SaleChannels { get; }

    /// <summary>Combo.</summary>
    public CombosService Combos { get; }

    /// <summary>Flash sale.</summary>
    public FlashSalesService FlashSales { get; }

    /// <summary>Voucher.</summary>
    public VouchersService Vouchers { get; }

    /// <summary>Chương trình chiết khấu/phụ thu theo đơn.</summary>
    public OrderAdjustmentProgramsService OrderAdjustmentPrograms { get; }

    /// <summary>Dữ liệu bán hàng FnB: khu vực/bàn, menu, món kèm, thuộc tính, phương thức thanh toán.</summary>
    public FnbService Fnb { get; }

    /// <summary>Đơn hàng FnB: tạo/sửa đơn, món, combo, chuyển/gộp/tách bàn.</summary>
    public FnbOrdersService FnbOrders { get; }

    /// <summary>Thanh toán đơn FnB: tạm tính, QR, thanh toán.</summary>
    public FnbPaymentsService FnbPayments { get; }

    /// <summary>
    /// Tạo client sở hữu riêng <see cref="HttpClient"/> (tự dispose khi client bị dispose).
    /// Dùng cho ứng dụng cần cấu hình handler riêng (proxy, chứng chỉ...).
    /// </summary>
    public static SofiPosFnBClient Create(SofiPosOptions options, HttpMessageHandler handler)
    {
        ArgumentNullException.ThrowIfNull(handler);
        return new SofiPosFnBClient(new HttpClient(handler, disposeHandler: true) { Timeout = Timeout.InfiniteTimeSpan }, options, ownsHttpClient: true);
    }

    /// <summary>Giải phóng HttpClient nếu client sở hữu nó.</summary>
    public void Dispose() => _ownedHttpClient?.Dispose();

    internal static SocketsHttpHandler CreateDefaultHandler() => new()
    {
        // Làm mới kết nối định kỳ để nhận thay đổi DNS khi dùng HttpClient lâu dài.
        PooledConnectionLifetime = TimeSpan.FromMinutes(5),
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli
    };

    private static HttpClient CreateDefaultHttpClient()
        // Timeout do SDK quản lý theo từng request (SofiPosOptions.Timeout).
        => new(CreateDefaultHandler(), disposeHandler: true) { Timeout = Timeout.InfiniteTimeSpan };
}
