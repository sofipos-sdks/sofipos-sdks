using System.Security.Cryptography;
using System.Text;
using SofiPOS.FnB.Webhooks;

namespace SofiPOS.FnB.Sdk.Tests;

public class WebhookTests
{
    private const string Secret = "whsec_test_secret_value";
    private static readonly DateTimeOffset Now = DateTimeOffset.FromUnixTimeSeconds(1_763_456_400);
    private static readonly string Timestamp = Now.ToUnixTimeSeconds().ToString();

    private const string OrderBody = "{\"id\":\"evt_01\",\"event\":\"order.created\",\"createdAt\":\"2026-11-18T15:30:00+07:00\","
        + "\"data\":{\"orderId\":\"0b6f2a9e-1c3d-4e5f-8a7b-9c0d1e2f3a4b\",\"code\":\"DH000123\",\"branchId\":\"6f1c2a1e-1d5b-4d0e-9a0a-2b1f6c1e9a11\"}}";

    private static string ExpectedSignature(string body)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Secret));
        return "sha256=" + Convert.ToHexString(hmac.ComputeHash(Encoding.UTF8.GetBytes(Timestamp + "." + body))).ToLowerInvariant();
    }

    [Fact]
    public void Signature_matches_independent_hmac()
    {
        Assert.Equal(ExpectedSignature(OrderBody), SofiPosWebhookVerifier.ComputeSignature(Secret, Now.ToUnixTimeSeconds(), OrderBody));
    }

    [Fact]
    public void Valid_signature_is_accepted_and_event_parsed()
    {
        var webhookEvent = SofiPosWebhookVerifier.VerifyAndParse(Secret, Timestamp, ExpectedSignature(OrderBody), OrderBody, now: Now);

        Assert.Equal(SofiPosWebhookEvents.OrderCreated, webhookEvent.Event);
        Assert.True(webhookEvent.IsOrderEvent);
        var data = webhookEvent.GetOrderData();
        Assert.Equal(Guid.Parse("0b6f2a9e-1c3d-4e5f-8a7b-9c0d1e2f3a4b"), data.OrderId);
        Assert.Equal("DH000123", data.Code);
        Assert.Equal(TimeSpan.FromHours(7), webhookEvent.CreatedAt.Offset);
    }

    [Fact]
    public void Uppercase_signature_and_byte_body_are_accepted()
    {
        Assert.True(SofiPosWebhookVerifier.IsValid(Secret, Timestamp, ExpectedSignature(OrderBody).ToUpperInvariant().Replace("SHA256=", "sha256="),
            Encoding.UTF8.GetBytes(OrderBody), now: Now));
    }

    [Fact]
    public void Tampered_body_is_rejected()
    {
        var tampered = OrderBody.Replace("DH000123", "DH999999");

        Assert.False(SofiPosWebhookVerifier.IsValid(Secret, Timestamp, ExpectedSignature(OrderBody), tampered, now: Now));
        var ex = Assert.Throws<SofiPosWebhookException>(() =>
            SofiPosWebhookVerifier.VerifyAndParse(Secret, Timestamp, ExpectedSignature(OrderBody), tampered, now: Now));
        Assert.Contains("Chữ ký", ex.Message);
    }

    [Fact]
    public void Wrong_secret_is_rejected()
    {
        Assert.False(SofiPosWebhookVerifier.IsValid("whsec_other", Timestamp, ExpectedSignature(OrderBody), OrderBody, now: Now));
    }

    [Fact]
    public void Old_timestamp_is_rejected_but_tolerance_can_be_changed()
    {
        var later = Now.AddMinutes(10);

        Assert.False(SofiPosWebhookVerifier.IsValid(Secret, Timestamp, ExpectedSignature(OrderBody), OrderBody, now: later));
        Assert.True(SofiPosWebhookVerifier.IsValid(Secret, Timestamp, ExpectedSignature(OrderBody), OrderBody, TimeSpan.FromMinutes(15), later));
    }

    [Theory]
    [InlineData(null, "sha256=abc")]
    [InlineData("not-a-number", "sha256=abc")]
    [InlineData("1763456400", null)]
    [InlineData("1763456400", "")]
    public void Missing_headers_are_rejected(string? timestamp, string? signature)
    {
        Assert.False(SofiPosWebhookVerifier.IsValid(Secret, timestamp, signature, OrderBody, now: Now));
    }

    [Fact]
    public void Change_event_data_is_parsed()
    {
        const string body = "{\"id\":\"evt_02\",\"event\":\"product.changed\",\"createdAt\":\"2026-11-18T08:30:00Z\",\"data\":{\"ids\":[\"a\",\"b\"],\"deletedIds\":[\"c\"]}}";

        var webhookEvent = SofiPosWebhookVerifier.Parse(body);
        var data = webhookEvent.GetChangeData();

        Assert.False(webhookEvent.IsOrderEvent);
        Assert.Equal(new[] { "a", "b" }, data.Ids);
        Assert.Equal(new[] { "c" }, data.DeletedIds);
    }

    [Fact]
    public void Invalid_json_throws_webhook_exception()
    {
        Assert.Throws<SofiPosWebhookException>(() => SofiPosWebhookVerifier.Parse("not json"));
        Assert.Throws<SofiPosWebhookException>(() => SofiPosWebhookVerifier.Parse("{\"id\":\"x\"}"));
    }
}
