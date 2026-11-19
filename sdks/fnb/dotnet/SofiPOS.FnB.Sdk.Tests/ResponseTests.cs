using System.Net;

namespace SofiPOS.FnB.Sdk.Tests;

public class ResponseTests
{
    [Fact]
    public async Task Order_response_is_deserialized_with_vietnam_time()
    {
        var handler = new MockHttpHandler().RespondJson(RequestTests.OrderJson);
        var client = TestClient.Create(handler);

        var order = await client.FnbOrders.GetAsync(Guid.NewGuid(), Guid.Parse("0b6f2a9e-1c3d-4e5f-8a7b-9c0d1e2f3a4b"));

        Assert.Equal("DH000123", order.Code);
        Assert.Equal(new DateTime(2026, 11, 18, 15, 30, 0).AddTicks(1234567), order.CreateDate);
        Assert.Equal(DateTimeKind.Unspecified, order.CreateDate.Kind);
        Assert.Equal(PaymentMethod.Cash, order.PaymentMethod);
    }

    [Fact]
    public async Task Utc_date_in_response_is_converted_to_vietnam_wall_clock()
    {
        var json = RequestTests.OrderJson.Replace("2026-11-18T15:30:00.1234567+07:00", "2026-11-18T08:30:00Z");
        var client = TestClient.Create(new MockHttpHandler().RespondJson(json));

        var order = await client.FnbOrders.GetAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.Equal(new DateTime(2026, 11, 18, 15, 30, 0), order.CreateDate);
    }

    [Fact]
    public async Task Paged_result_is_deserialized()
    {
        const string body = "{\"metaData\":{\"currentPage\":2,\"totalPages\":3,\"pageSize\":1,\"totalCount\":3,\"hasPrevious\":true,\"hasNext\":true},"
            + "\"items\":[{\"saleChannelId\":\"GiaoHang\",\"saleChannelName\":\"Giao hàng\",\"isActive\":true,\"priority\":2,\"isECommerce\":1}]}";
        var client = TestClient.Create(new MockHttpHandler().RespondJson(body));

        var page = await client.SaleChannels.ListAsync();

        Assert.Equal(2, page.MetaData.CurrentPage);
        Assert.True(page.MetaData.HasNext);
        var channel = Assert.Single(page.Items);
        Assert.Equal("Giao hàng", channel.SaleChannelName);
        Assert.Equal(SaleChannelECommerceType.ECommerceWithPayment, channel.IsECommerce);
    }

    [Fact]
    public async Task ListAll_follows_pages_until_has_next_is_false()
    {
        string Page(int n, bool hasNext) =>
            "{\"metaData\":{\"currentPage\":" + n + ",\"totalPages\":2,\"pageSize\":1,\"totalCount\":2,\"hasPrevious\":false,\"hasNext\":" + (hasNext ? "true" : "false") + "},"
            + "\"items\":[{\"saleChannelId\":\"K" + n + "\",\"saleChannelName\":\"K" + n + "\"}]}";
        var handler = new MockHttpHandler().RespondJson(Page(1, true)).RespondJson(Page(2, false));
        var client = TestClient.Create(handler);

        var all = new List<string>();
        await foreach (var channel in client.SaleChannels.ListAllAsync()) all.Add(channel.SaleChannelId ?? string.Empty);

        Assert.Equal(new[] { "K1", "K2" }, all);
        Assert.Contains("pageNumber=1", handler.Requests[0].Uri.Query);
        Assert.Contains("pageNumber=2", handler.Requests[1].Uri.Query);
    }

    [Fact]
    public async Task Status_201_with_body_is_success()
    {
        var client = TestClient.Create(new MockHttpHandler().Respond(HttpStatusCode.Created, RequestTests.OrderJson));

        var order = await client.FnbOrders.CreateAsync(Guid.NewGuid(), new FnbOrderCreateRequest
        {
            RequestId = Guid.NewGuid(),
            Items = { new FnbOrderItemRequest { ProductId = Guid.NewGuid(), Quantity = 1 } }
        });

        Assert.Equal("DH000123", order.Code);
    }

    [Fact]
    public async Task Status_204_is_success_for_delete()
    {
        var handler = new MockHttpHandler().Respond(HttpStatusCode.NoContent);
        var client = TestClient.Create(handler);

        await client.Products.DeleteAsync(Guid.NewGuid());

        Assert.Equal(HttpMethod.Post, handler.Requests[0].Method);
        Assert.EndsWith("/delete", handler.Requests[0].Uri.AbsolutePath);
    }

    [Fact]
    public async Task Empty_body_when_data_is_expected_throws()
    {
        var client = TestClient.Create(new MockHttpHandler().Respond(HttpStatusCode.NoContent));

        var ex = await Assert.ThrowsAsync<SofiPosApiException>(() => client.FnbOrders.GetAsync(Guid.NewGuid(), Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.NoContent, ex.StatusCode);
    }

    [Fact]
    public async Task Invalid_json_throws_api_exception_with_body()
    {
        var client = TestClient.Create(new MockHttpHandler().RespondJson("<html>ok</html>"));

        var ex = await Assert.ThrowsAsync<SofiPosApiException>(() => client.FnbOrders.GetAsync(Guid.NewGuid(), Guid.NewGuid()));

        Assert.Equal("<html>ok</html>", ex.ResponseBody);
        Assert.IsAssignableFrom<System.Text.Json.JsonException>(ex.InnerException);
    }

    [Fact]
    public async Task Boolean_response_is_read()
    {
        var client = TestClient.Create(new MockHttpHandler().RespondJson("true"));

        var paid = await client.FnbPayments.CheckQrTransactionAsync(Guid.NewGuid(), Guid.NewGuid(), "PaymentVietQr");

        Assert.True(paid);
    }
}
