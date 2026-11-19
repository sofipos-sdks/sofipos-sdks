using System.Text.Json;
using SofiPOS.FnB.Models;

namespace SofiPOS.FnB.Sdk.Tests;

public class RequestTests
{
    [Fact]
    public async Task Request_has_partner_key_header_and_json_accept()
    {
        var handler = new MockHttpHandler().RespondJson("[]");
        var client = TestClient.Create(handler);

        await client.Lookups.GetBranchesAsync();

        var request = Assert.Single(handler.Requests);
        Assert.Equal(TestClient.ApiKey, request.Headers["X-Partner-Key"]);
        Assert.Contains("application/json", request.Headers["Accept"]);
    }

    [Fact]
    public async Task Api_key_provider_is_called_for_each_request()
    {
        var handler = new MockHttpHandler().RespondJson("[]").RespondJson("[]");
        var keys = new Queue<string>(new[] { "tp_key_1", "tp_key_2" });
        var client = TestClient.Create(handler, o =>
        {
            o.ApiKey = null;
            o.ApiKeyProvider = _ => ValueTask.FromResult(keys.Dequeue());
        });

        await client.Lookups.GetBranchesAsync();
        await client.Lookups.GetBranchesAsync();

        Assert.Equal("tp_key_1", handler.Requests[0].Headers["X-Partner-Key"]);
        Assert.Equal("tp_key_2", handler.Requests[1].Headers["X-Partner-Key"]);
    }

    [Theory]
    [InlineData("https://api.test.local")]
    [InlineData("https://api.test.local/")]
    [InlineData("https://api.test.local/api/partner/v1")]
    [InlineData("https://api.test.local/api/partner/v1/")]
    public async Task Base_url_is_combined_with_partner_prefix_once(string baseUrl)
    {
        var handler = new MockHttpHandler().RespondJson("[]");
        var client = TestClient.Create(handler, o => o.BaseUrl = baseUrl);

        await client.Lookups.GetMembershipRanksAsync();

        Assert.Equal("https://api.test.local/api/partner/v1/lookups/membership-ranks", handler.Requests[0].Uri.ToString());
    }

    [Fact]
    public async Task List_query_string_contains_paging_and_encoded_filters()
    {
        var handler = new MockHttpHandler().RespondJson(EmptyPage);
        var client = TestClient.Create(handler);

        await client.Products.ListAsync(new ProductListQuery { Keyword = "cà phê & sữa", PageNumber = 2, PageSize = 50 });

        Assert.Equal(HttpMethod.Get, handler.Requests[0].Method);
        Assert.Equal("/api/partner/v1/products?pageNumber=2&pageSize=50&keyword=c%C3%A0%20ph%C3%AA%20%26%20s%E1%BB%AFa",
            handler.Requests[0].Uri.PathAndQuery);
    }

    [Fact]
    public async Task Null_filters_are_not_sent()
    {
        var handler = new MockHttpHandler().RespondJson(EmptyPage);
        var client = TestClient.Create(handler);

        await client.FnbOrders.ListAsync(new FnbOrderListQuery
        {
            BranchId = Guid.Parse("5a4b3c2d-1e0f-4a9b-8c7d-6e5f4a3b2c1d"),
            FromDate = new DateTime(2026, 11, 1, 8, 30, 0),
            IsPaid = false
        });

        Assert.Equal("/api/partner/v1/fnb/orders?pageNumber=1&pageSize=15&branchId=5a4b3c2d-1e0f-4a9b-8c7d-6e5f4a3b2c1d"
            + "&fromDate=2026-11-01T08%3A30%3A00&isPaid=false", handler.Requests[0].Uri.PathAndQuery);
    }

    [Fact]
    public async Task Route_segments_are_escaped_and_methods_match_api()
    {
        var handler = new MockHttpHandler()
            .RespondJson(SaleChannelJson)
            .Respond(System.Net.HttpStatusCode.OK, "true")
            .RespondJson("true");
        var client = TestClient.Create(handler);

        await client.SaleChannels.UpdateAsync("Giao Hang/1", new SaleChannelUpdateRequest { IsActive = true, Priority = 2 });
        await client.Products.DeleteAsync(Guid.Parse("6f1c2a1e-1d5b-4d0e-9a0a-2b1f6c1e9a11"));
        await client.FnbPayments.CheckQrTransactionAsync(Guid.NewGuid(), Guid.Parse("0b6f2a9e-1c3d-4e5f-8a7b-9c0d1e2f3a4b"), "PaymentVietQr");

        Assert.Equal(HttpMethod.Put, handler.Requests[0].Method);
        Assert.Equal("/api/partner/v1/sale-channels/Giao%20Hang%2F1", handler.Requests[0].Uri.AbsolutePath);
        Assert.Contains("\"saleChannelId\":\"Giao Hang/1\"", handler.Requests[0].Body);

        Assert.Equal(HttpMethod.Post, handler.Requests[1].Method);
        Assert.Equal("/api/partner/v1/products/6f1c2a1e-1d5b-4d0e-9a0a-2b1f6c1e9a11/delete", handler.Requests[1].Uri.AbsolutePath);
        Assert.Null(handler.Requests[1].Body);

        Assert.Equal("/api/partner/v1/fnb/orders/0b6f2a9e-1c3d-4e5f-8a7b-9c0d1e2f3a4b/qr/check/PaymentVietQr", handler.Requests[2].Uri.AbsolutePath);
    }

    [Fact]
    public async Task Create_order_body_uses_api_field_names_and_omits_nulls()
    {
        var handler = new MockHttpHandler().RespondJson(OrderJson);
        var client = TestClient.Create(handler);
        var requestId = Guid.Parse("6c2f1b0e-0d8f-4a43-9d34-1a2b3c4d5e6f");
        var productId = Guid.Parse("11111111-2222-3333-4444-555555555555");

        await client.FnbOrders.CreateAsync(Guid.Parse("5a4b3c2d-1e0f-4a9b-8c7d-6e5f4a3b2c1d"), new FnbOrderCreateRequest
        {
            RequestId = requestId,
            SaleChannelId = "TaiCho",
            Note = "Khách dị ứng đậu phộng",
            Items =
            {
                new FnbOrderItemRequest
                {
                    ProductId = productId,
                    Quantity = 2,
                    Attributes = { new AttributeValue("duong", "Ít đường") },
                    AddOns = { new FnbOrderAddOnRequest { ProductId = productId } }
                }
            }
        });

        var request = handler.Requests[0];
        Assert.Equal(HttpMethod.Post, request.Method);
        Assert.Equal("/api/partner/v1/fnb/orders?branchId=5a4b3c2d-1e0f-4a9b-8c7d-6e5f4a3b2c1d", request.Uri.PathAndQuery);
        Assert.StartsWith("application/json", request.ContentType);

        using var json = JsonDocument.Parse(request.Body!);
        var root = json.RootElement;
        Assert.Equal(requestId, root.GetProperty("requestId").GetGuid());
        Assert.Equal("Khách dị ứng đậu phộng", root.GetProperty("note").GetString());
        Assert.False(root.TryGetProperty("tableId", out _));
        Assert.False(root.TryGetProperty("customerId", out _));
        var item = root.GetProperty("items")[0];
        Assert.Equal(2, item.GetProperty("quantity").GetDecimal());
        Assert.False(item.TryGetProperty("unitId", out _));
        Assert.Equal("Ít đường", item.GetProperty("attributes")[0].GetProperty("value").GetString());
        Assert.Equal(1, item.GetProperty("addOns")[0].GetProperty("quantity").GetDecimal());
    }

    [Fact]
    public async Task Explicit_null_is_sent_where_api_needs_it()
    {
        var handler = new MockHttpHandler().RespondJson(OrderJson).RespondJson(OrderJson);
        var client = TestClient.Create(handler);
        var orderId = Guid.NewGuid();

        await client.FnbOrders.SetCustomerAsync(Guid.NewGuid(), orderId, null);
        await client.FnbOrders.UpdateProductUnitAsync(Guid.NewGuid(), orderId, Guid.NewGuid(), null);

        Assert.Equal("{\"customerId\":null}", handler.Requests[0].Body);
        Assert.Equal("{\"unitId\":null}", handler.Requests[1].Body);
    }

    [Fact]
    public async Task Payment_body_serializes_enums_as_numbers_and_discount_none()
    {
        var handler = new MockHttpHandler().RespondJson(OrderJson);
        var client = TestClient.Create(handler);

        await client.FnbPayments.PayAsync(Guid.NewGuid(), Guid.NewGuid(), new FnbPaymentRequest
        {
            Discount = OrderDiscountRequest.None,
            Voucher = new OrderVoucherRequest("ABC123"),
            PaymentMethod = PaymentMethod.Bank,
            PaymentProvider = "PaymentVietQr",
            CustomerPayment = 150000
        });

        using var json = JsonDocument.Parse(handler.Requests[0].Body!);
        var root = json.RootElement;
        Assert.Equal(HttpMethod.Put, handler.Requests[0].Method);
        Assert.Equal(0, root.GetProperty("discount").GetProperty("discount").GetDecimal());
        Assert.False(root.TryGetProperty("surcharge", out _));
        Assert.Equal("ABC123", root.GetProperty("voucher").GetProperty("voucherCode").GetString());
        Assert.Equal(2, root.GetProperty("paymentMethod").GetInt32());
        Assert.Equal(150000, root.GetProperty("customerPayment").GetDecimal());
    }

    [Fact]
    public async Task Catalog_request_serializes_dates_times_and_vat_names()
    {
        var handler = new MockHttpHandler().RespondJson("{\"id\":\"8c1e8c1e-8c1e-8c1e-8c1e-8c1e8c1e8c1e\"}");
        var client = TestClient.Create(handler);

        await client.OrderAdjustmentPrograms.CreateAsync(new OrderAdjustmentProgramRequest
        {
            Title = "Phụ thu ngày lễ",
            Type = OrderAdjustmentType.Surcharge,
            ValueType = DiscountType.Percent,
            Value = 10,
            FromDate = new DateTime(2026, 12, 24),
            DaysOfWeek = { DayOfWeek.Saturday, DayOfWeek.Sunday },
            TimeSlots = { new TimeSlot { StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(23, 0) } }
        });

        using var json = JsonDocument.Parse(handler.Requests[0].Body!);
        var root = json.RootElement;
        Assert.Equal(2, root.GetProperty("type").GetInt32());
        Assert.Equal("2026-12-24T00:00:00", root.GetProperty("fromDate").GetString());
        Assert.False(root.TryGetProperty("isApplyBeforeVat", out _));
        Assert.True(root.TryGetProperty("isApplyBeforeVAT", out _));
        Assert.Equal("[6,0]", root.GetProperty("daysOfWeek").GetRawText());
        Assert.Equal("18:00:00", root.GetProperty("timeSlots")[0].GetProperty("startTime").GetString());
    }

    [Fact]
    public async Task Utc_datetime_is_sent_as_vietnam_time()
    {
        var handler = new MockHttpHandler().RespondJson(EmptyPage);
        var client = TestClient.Create(handler);

        await client.Customers.CreateAsync(new CustomerRequest
        {
            CustomerName = "An",
            CustomerPhone = "0901234567",
            BirthDate = new DateTime(1990, 5, 19, 17, 0, 0, DateTimeKind.Utc)
        }).ContinueWith(_ => { });

        Assert.Contains("\"birthDate\":\"1990-05-20T00:00:00\"", handler.Requests[0].Body);
    }

    internal const string EmptyPage = "{\"metaData\":{\"currentPage\":1,\"totalPages\":0,\"pageSize\":15,\"totalCount\":0,\"hasPrevious\":false,\"hasNext\":false},\"items\":[]}";

    internal const string SaleChannelJson = "{\"saleChannelId\":\"GiaoHang\",\"saleChannelName\":\"Giao hàng\",\"companyId\":\"6f1c2a1e-1d5b-4d0e-9a0a-2b1f6c1e9a11\",\"isActive\":true,\"priority\":2,\"canDelete\":true,\"isECommerce\":0}";

    internal const string OrderJson = "{\"id\":\"0b6f2a9e-1c3d-4e5f-8a7b-9c0d1e2f3a4b\",\"code\":\"DH000123\",\"createDate\":\"2026-11-18T15:30:00.1234567+07:00\",\"paymentMethod\":1,\"products\":[],\"combos\":[]}";
}
