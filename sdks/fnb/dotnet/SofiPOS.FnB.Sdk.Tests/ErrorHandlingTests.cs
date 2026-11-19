using System.Net;

namespace SofiPOS.FnB.Sdk.Tests;

public class ErrorHandlingTests
{
    [Fact]
    public async Task Bad_request_reads_sofipos_error_body()
    {
        const string body = "{\"message\":\"Dữ liệu không hợp lệ\",\"codeError\":1001,\"details\":[{\"field\":\"items[0].quantity\",\"message\":\"Số lượng phải lớn hơn 0\"}]}";
        var client = TestClient.Create(new MockHttpHandler().Respond(HttpStatusCode.BadRequest, body,
            r => r.Headers.Add("trace-id", "00-abc-01")));

        var ex = await Assert.ThrowsAsync<SofiPosApiException>(() => client.FnbOrders.GetAsync(Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.BadRequest, ex.StatusCode);
        Assert.Equal("Dữ liệu không hợp lệ", ex.Message);
        Assert.Equal(1001, ex.ErrorCode);
        Assert.Equal("00-abc-01", ex.TraceId);
        Assert.True(ex.IsValidationError);
        var error = Assert.Single(ex.Errors);
        Assert.Equal("items[0].quantity", error.Field);
        Assert.Equal("Số lượng phải lớn hơn 0", error.Message);
        Assert.Equal(body, ex.ResponseBody);
    }

    [Fact]
    public async Task Problem_details_validation_errors_are_read()
    {
        const string body = "{\"title\":\"One or more validation errors occurred.\",\"status\":400,\"errors\":{\"Note\":[\"Quá dài\"]}}";
        var client = TestClient.Create(new MockHttpHandler().Respond(HttpStatusCode.BadRequest, body));

        var ex = await Assert.ThrowsAsync<SofiPosApiException>(() => client.FnbOrders.UpdateNoteAsync(Guid.NewGuid(), "x"));

        Assert.Equal("One or more validation errors occurred.", ex.Message);
        Assert.Equal(new SofiPosValidationError("Note", "Quá dài"), Assert.Single(ex.Errors));
        Assert.Null(ex.ErrorCode);
    }

    [Fact]
    public async Task Json_string_body_becomes_message()
    {
        var client = TestClient.Create(new MockHttpHandler().Respond(HttpStatusCode.BadRequest, "\"Bàn đang có đơn khác\""));

        var ex = await Assert.ThrowsAsync<SofiPosApiException>(() => client.FnbOrders.ChangeTableAsync(Guid.NewGuid(), Guid.NewGuid()));

        Assert.Equal("Bàn đang có đơn khác", ex.Message);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, "Partner Key không hợp lệ")]
    [InlineData(HttpStatusCode.Forbidden, "Không có quyền")]
    [InlineData(HttpStatusCode.NotFound, "Dữ liệu không tồn tại")]
    [InlineData(HttpStatusCode.Conflict, "thay đổi đồng thời")]
    [InlineData((HttpStatusCode)429, "quá nhiều")]
    [InlineData(HttpStatusCode.InternalServerError, "đang lỗi")]
    public async Task Error_without_body_uses_default_message(HttpStatusCode status, string expected)
    {
        var client = TestClient.Create(new MockHttpHandler().Respond(status), o => o.MaxRetries = 0);

        var ex = await Assert.ThrowsAsync<SofiPosApiException>(() => client.Lookups.GetBranchesAsync());

        Assert.Equal(status, ex.StatusCode);
        Assert.Contains(expected, ex.Message);
    }

    [Fact]
    public async Task Status_flags_are_set()
    {
        var client = TestClient.Create(new MockHttpHandler()
            .Respond(HttpStatusCode.Unauthorized)
            .Respond(HttpStatusCode.NotFound)
            .Respond(HttpStatusCode.InternalServerError));

        var unauthorized = await Assert.ThrowsAsync<SofiPosApiException>(() => client.Lookups.GetBranchesAsync());
        var notFound = await Assert.ThrowsAsync<SofiPosApiException>(() => client.Lookups.GetBranchesAsync());
        var server = await Assert.ThrowsAsync<SofiPosApiException>(() => client.Lookups.GetBranchesAsync());

        Assert.True(unauthorized.IsUnauthorized);
        Assert.True(notFound.IsNotFound);
        Assert.True(server.IsTransient);
        Assert.False(notFound.IsTransient);
    }

    [Fact]
    public async Task Non_json_error_body_keeps_raw_body()
    {
        var client = TestClient.Create(new MockHttpHandler().Respond(HttpStatusCode.BadGateway, "<html>502 Bad Gateway</html>"), o => o.MaxRetries = 0);

        var ex = await Assert.ThrowsAsync<SofiPosApiException>(() => client.Lookups.GetBranchesAsync());

        Assert.Contains("đang lỗi", ex.Message);
        Assert.Equal("<html>502 Bad Gateway</html>", ex.ResponseBody);
    }

    [Fact]
    public async Task Exception_never_contains_api_key()
    {
        var client = TestClient.Create(new MockHttpHandler().Respond(HttpStatusCode.Unauthorized, "{\"message\":\"Sai key\"}"));

        var ex = await Assert.ThrowsAsync<SofiPosApiException>(() => client.Lookups.GetBranchesAsync());

        Assert.DoesNotContain(TestClient.ApiKey, ex.ToString());
        Assert.DoesNotContain(TestClient.ApiKey, ex.ResponseBody ?? string.Empty);
        Assert.DoesNotContain("SECRETPART", ex.ToString());
    }

    [Fact]
    public void Options_to_string_does_not_contain_api_key()
    {
        var options = new SofiPOS.FnB.Configuration.SofiPosOptions { BaseUrl = TestClient.BaseUrl, ApiKey = TestClient.ApiKey };

        Assert.DoesNotContain("SECRETPART", options.ToString());
    }

    [Fact]
    public void Missing_configuration_is_rejected()
    {
        Assert.Throws<ArgumentException>(() => new SofiPosFnBClient(new HttpClient(), new SofiPOS.FnB.Configuration.SofiPosOptions { ApiKey = "x" }));
        Assert.Throws<ArgumentException>(() => new SofiPosFnBClient(new HttpClient(), new SofiPOS.FnB.Configuration.SofiPosOptions { BaseUrl = TestClient.BaseUrl }));
    }
}
