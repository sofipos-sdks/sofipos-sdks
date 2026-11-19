# SofiPOS FnB SDK cho .NET

Thư viện chính thức để đối tác kết nối với **SofiPOS FnB Partner API** (`/api/partner/v1`) từ ứng dụng .NET: đọc thực đơn, khu vực/bàn, tạo và cập nhật đơn tại bàn, thanh toán, quản lý danh mục (món, combo, flash sale, chính sách giá, voucher, khách hàng, kênh bán, mã QR gọi món) và kiểm tra webhook.

- Gói NuGet: `SofiPOS.FnB.Sdk`
- Namespace gốc: `SofiPOS.FnB`
- Nền tảng: .NET 8 trở lên
- Phụ thuộc duy nhất: `Microsoft.Extensions.Http` (cho `IHttpClientFactory` và DI)

## Cài đặt

Để dùng trực tiếp source trong repository, xem [Bắt đầu nhanh](https://github.com/sofipos-sdks/sofipos-sdks/blob/main/docs/getting-started.md). Lệnh NuGet bên dưới áp dụng khi package đã được phát hành; version trong source hiện là `1.0.0`.

```bash
dotnet add package SofiPOS.FnB.Sdk
```

Khi chưa phát hành lên NuGet.org, cài từ file `.nupkg` được SofiPOS cung cấp:

```bash
dotnet nuget add source D:\packages --name sofipos-local
dotnet add package SofiPOS.FnB.Sdk --version 1.0.0 --source sofipos-local
```

## Cấu hình

| Thuộc tính | Bắt buộc | Mặc định | Ý nghĩa |
|---|---|---|---|
| `BaseUrl` | Có | | Địa chỉ API do SofiPOS cấp, ví dụ `https://api.example.com`. SDK tự thêm `/api/partner/v1/`. |
| `ApiKey` | Có (hoặc `ApiKeyProvider`) | | Partner Key của cửa hàng (`tp_...`), lấy ở màn hình **Hệ thống > Kết nối bên thứ 3**. |
| `ApiKeyProvider` | Không | | Hàm trả Partner Key cho mỗi request, dùng khi key nằm trong kho bí mật hoặc thay đổi theo thời gian. |
| `Timeout` | Không | 30 giây | Thời gian chờ tối đa của một lần gọi. |
| `MaxRetries` | Không | 2 | Số lần thử lại cho request **GET** khi lỗi mạng, timeout, 429, 502, 503, 504. |

Partner Key được gửi trong header `X-Partner-Key`. Không ghi key vào mã nguồn; hãy đọc từ biến môi trường, User Secrets hoặc kho bí mật. SDK không ghi log và không đưa key vào nội dung exception.

## Dùng nhanh (không cần DI)

```csharp
using SofiPOS.FnB;
using SofiPOS.FnB.Configuration;
using SofiPOS.FnB.Models;

var client = new SofiPosFnBClient(new SofiPosOptions
{
    BaseUrl = "https://api.example.com",
    ApiKey = Environment.GetEnvironmentVariable("SOFIPOS_API_KEY")
});

var branches = await client.Lookups.GetBranchesAsync();
var branchId = branches.First(b => b.IsActive).Id;

var menu = await client.Fnb.GetMenuAsync(branchId, saleChannelId: "TaiCho");
var areas = await client.Fnb.GetAreasAsync(branchId);
```

Hãy tạo client **một lần** rồi dùng lại (client an toàn khi gọi đồng thời). Khi không truyền `HttpClient`, SDK dùng chung một `HttpClient` nội bộ có làm mới kết nối định kỳ, nên không bị cạn socket hay giữ DNS cũ.

## Dùng với ASP.NET Core DI

```csharp
using SofiPOS.FnB.Extensions;

builder.Services.AddSofiPosFnB(options =>
{
    options.BaseUrl = builder.Configuration["SofiPOS:BaseUrl"]!;
    options.ApiKey = builder.Configuration["SofiPOS:ApiKey"];
});

// Inject trực tiếp
public class MenuSync(SofiPosFnBClient sofipos)
{
    public Task<FnbMenu> LoadAsync(Guid branchId, CancellationToken ct) => sofipos.Fnb.GetMenuAsync(branchId, "TaiCho", ct);
}
```

`AddSofiPosFnB` trả về `IHttpClientBuilder`, nên có thể gắn thêm handler (proxy, logging, Polly...) như mọi typed client khác.

### Nhiều cửa hàng (nhiều Partner Key)

Mỗi cửa hàng có một Partner Key riêng. Không đổi key trên một client dùng chung; hãy tạo client theo cửa hàng bằng `ISofiPosFnBClientFactory`:

```csharp
builder.Services.AddSofiPosFnB(o => o.BaseUrl = "https://api.example.com");

public class OrderPusher(ISofiPosFnBClientFactory factory, IStoreKeyStore keys)
{
    public async Task PushAsync(string storeId, FnbOrderCreateRequest order, CancellationToken ct)
    {
        var client = factory.CreateClient(await keys.GetPartnerKeyAsync(storeId, ct));
        await client.FnbOrders.CreateAsync(order, cancellationToken: ct);
    }
}
```

Client tạo từ factory dùng `HttpClient` do `IHttpClientFactory` quản lý, nên tạo theo từng request vẫn rẻ.

## Các nhóm chức năng

| Thuộc tính trên client | Endpoint | Chức năng |
|---|---|---|
| `Lookups` | `lookups/*` | Chi nhánh, hạng thành viên |
| `Products` | `products` | Món/hàng hóa, đơn vị tính; tạo, sửa, xóa |
| `Combos` | `combos` | Combo |
| `FlashSales` | `flashsales` | Flash sale |
| `PricingPolicies` | `pricing-policies` | Chính sách giá, món thêm theo chính sách |
| `Vouchers` | `vouchers` | Voucher, danh sách mã, bật/tắt |
| `OrderAdjustmentPrograms` | `order-adjustment-programs` | Chương trình giảm giá/phụ thu theo đơn |
| `Customers` | `customers` | Khách hàng |
| `SaleChannels` | `sale-channels` | Kênh bán |
| `QrOrderCodes` | `qr-order-codes` | Mã QR gọi món tại bàn, ảnh QR |
| `Fnb` | `fnb/areas`, `fnb/menu`, `fnb/products/*`, `fnb/payment-options` | Khu vực/bàn, thực đơn có giá theo kênh, món thêm, thuộc tính, phương thức thanh toán |
| `FnbOrders` | `fnb/orders` | Tạo đơn, xem đơn, sửa ghi chú/khách, thêm/sửa/hủy món và combo, đổi bàn, gộp đơn, tách món |
| `FnbPayments` | `fnb/orders/{id}/payment`, `fnb/orders/{id}/qr` | Xem trước tiền, tạo QR chuyển khoản, kiểm tra giao dịch, thanh toán |

Mọi phương thức đều bất đồng bộ, có hậu tố `Async` và nhận `CancellationToken`.

### Phân trang

```csharp
var page = await client.Products.ListAsync(new ProductListQuery { Keyword = "cà phê", PageNumber = 1, PageSize = 50 });
Console.WriteLine($"{page.MetaData.TotalCount} món, trang {page.MetaData.CurrentPage}/{page.MetaData.TotalPages}");

// Hoặc duyệt toàn bộ, SDK tự gọi các trang tiếp theo
await foreach (var product in client.Products.ListAllAsync(new ProductListQuery { PageSize = 100 }))
{
    // ...
}
```

## Tạo đơn và thanh toán

```csharp
var requestId = Guid.NewGuid();   // Lưu lại cùng đơn phía đối tác

var order = await client.FnbOrders.CreateAsync(new FnbOrderCreateRequest
{
    RequestId = requestId,
    SaleChannelId = "TaiCho",
    TableId = tableId,
    Note = "Khách dị ứng đậu phộng",
    Items =
    {
        new FnbOrderItemRequest
        {
            ProductId = productId,
            Quantity = 2,
            Attributes = { new AttributeValue(sugarAttribute.Id!, "Ít đường") },   // từ Fnb.GetProductAttributesAsync
            AddOns = { new FnbOrderAddOnRequest { ProductId = toppingId, Quantity = 1 } }
        }
    }
}, branchId);

var preview = await client.FnbPayments.PreviewAsync(order.Id, new FnbPaymentRequest
{
    Voucher = new OrderVoucherRequest("GIAM20K")
});

var paid = await client.FnbPayments.PayAsync(order.Id, new FnbPaymentRequest
{
    Voucher = new OrderVoucherRequest("GIAM20K"),
    PaymentMethod = PaymentMethod.Cash,
    CustomerPayment = preview.TotalAmount
});
```

### Chống tạo trùng đơn

SDK **không tự gửi lại** POST/PUT (tạo đơn, thanh toán, hủy...) vì có thể tạo dữ liệu hai lần. Khi tạo đơn bị lỗi mạng hoặc timeout, hãy gửi lại **với cùng `RequestId`**: SofiPOS trả về đơn đã tạo thay vì tạo đơn mới. Các API khác không có khóa chống trùng; trước khi gửi lại, hãy đọc lại dữ liệu (`GetAsync`) để kiểm tra thao tác đã thành công chưa.

### Giờ Việt Nam

SofiPOS lưu và trả thời gian theo giờ Việt Nam (UTC+7). SDK đọc mọi trường `DateTime` thành giờ Việt Nam (`DateTimeKind.Unspecified`). Khi gửi lên, `DateTime` có `Kind = Utc` hoặc `Local` được đổi sang giờ Việt Nam; `Unspecified` được coi là đã là giờ Việt Nam.

## Xử lý lỗi

Mọi phản hồi HTTP không thành công được ném ra dưới dạng `SofiPosApiException`:

```csharp
using SofiPOS.FnB.Exceptions;

try
{
    await client.FnbOrders.CreateAsync(request, branchId);
}
catch (SofiPosApiException ex) when (ex.IsValidationError)
{
    foreach (var error in ex.Errors) Console.WriteLine($"{error.Field}: {error.Message}");
}
catch (SofiPosApiException ex) when (ex.IsUnauthorized)
{
    // Partner Key sai, đã bị thay hoặc đối tác bị tắt
}
catch (SofiPosApiException ex)
{
    logger.LogWarning("SofiPOS lỗi {Status} {Code}: {Message} (trace-id {TraceId})",
        (int)ex.StatusCode, ex.ErrorCode, ex.Message, ex.TraceId);
}
catch (TimeoutException)
{
    // Vượt quá SofiPosOptions.Timeout
}
```

| Thuộc tính | Ý nghĩa |
|---|---|
| `StatusCode` | Mã HTTP |
| `ErrorCode` | Mã lỗi nghiệp vụ của SofiPOS (`codeError`), nếu có |
| `Message` | Thông báo lỗi (tiếng Việt) |
| `TraceId` | Giá trị header `trace-id`, gửi cho SofiPOS khi cần hỗ trợ |
| `Errors` | Danh sách lỗi theo trường (`Field`, `Message`) |
| `ResponseBody` | Nội dung phản hồi gốc (tối đa 4000 ký tự) |
| `IsUnauthorized`, `IsNotFound`, `IsConflict`, `IsValidationError`, `IsTransient` | Cờ tiện kiểm tra |

Hết thời gian chờ ném `TimeoutException`; hủy bằng `CancellationToken` ném `OperationCanceledException`.

### Thử lại tự động

Chỉ request **GET** được thử lại (mặc định 2 lần) khi lỗi mạng, timeout hoặc mã 429/502/503/504, có tôn trọng header `Retry-After`. Lỗi 500 không được thử lại. Đặt `MaxRetries = 0` để tắt.

## Webhook

SofiPOS gửi webhook (POST JSON) tới URL đối tác đã cấu hình, ký bằng HMAC-SHA256 với secret `whsec_...` của cửa hàng. SDK chỉ cung cấp hàm kiểm tra chữ ký và đọc sự kiện; không chạy web server và không phụ thuộc ASP.NET Core.

Header gửi kèm: `X-Webhook-Id`, `X-Webhook-Event`, `X-Webhook-Timestamp`, `X-Webhook-Signature`.

```csharp
using SofiPOS.FnB.Webhooks;

app.MapPost("/sofipos/webhook", async (HttpRequest request, SofiPosFnBClient sofipos) =>
{
    using var buffer = new MemoryStream();
    await request.Body.CopyToAsync(buffer);   // Phải dùng nguyên văn body, không parse rồi serialize lại

    SofiPosWebhookEvent webhookEvent;
    try
    {
        webhookEvent = SofiPosWebhookVerifier.VerifyAndParse(
            secret: builder.Configuration["SofiPOS:WebhookSecret"]!,
            timestampHeader: request.Headers[SofiPosWebhookHeaders.Timestamp],
            signatureHeader: request.Headers[SofiPosWebhookHeaders.Signature],
            rawBody: buffer.ToArray());
    }
    catch (SofiPosWebhookException)
    {
        return Results.Unauthorized();
    }

    if (webhookEvent.IsOrderEvent)
    {
        var data = webhookEvent.GetOrderData();
        var order = await sofipos.FnbOrders.GetAsync(data.OrderId);   // Webhook chỉ báo thay đổi; lấy dữ liệu mới nhất qua API
    }
    else if (webhookEvent.Event == SofiPosWebhookEvents.ProductChanged)
    {
        var change = webhookEvent.GetChangeData();   // change.Ids, change.DeletedIds
    }

    return Results.Ok();
});
```

- Chữ ký: `sha256=` + hex chữ thường của `HMAC_SHA256(secret, timestamp + "." + rawBody)`.
- Mặc định từ chối webhook lệch quá 5 phút (chống gửi lại); đổi bằng tham số `tolerance`.
- So sánh chữ ký bằng `CryptographicOperations.FixedTimeEquals`.
- Trả 2xx nhanh rồi xử lý sau; SofiPOS gửi lại khi không nhận được 2xx. Dùng `webhookEvent.Id` để bỏ qua sự kiện trùng.
- Danh sách tên sự kiện ở `SofiPosWebhookEvents` (`ping`, `order.created`, `order.updated`, `product.changed`, ...).

## Ví dụ

Thư mục `examples/SofiPOS.FnB.Example`:

```bash
# Kiểm tra chữ ký webhook, không cần mạng
dotnet run --project examples/SofiPOS.FnB.Example -- webhook

# Gọi API thật, chỉ đọc dữ liệu (chi nhánh, bàn, thực đơn, đơn gần đây)
export SOFIPOS_BASE_URL=https://api.example.com
export SOFIPOS_API_KEY=tp_...
dotnet run --project examples/SofiPOS.FnB.Example

# Tạo thêm một đơn thử (ghi dữ liệu, chỉ dùng với cửa hàng thử nghiệm)
dotnet run --project examples/SofiPOS.FnB.Example -- create-order
```

Trên Windows PowerShell dùng `$env:SOFIPOS_BASE_URL = "..."` thay cho `export`.

## Build từ mã nguồn

```bash
dotnet restore
dotnet build -c Release
dotnet test -c Release
dotnet pack SofiPOS.FnB.Sdk -c Release -o artifacts
```

Gói được tạo tại `artifacts/SofiPOS.FnB.Sdk.1.0.0.nupkg` (kèm `.snupkg` chứa symbol).

## Phát hành gói

SofiPOS tự phát hành; SDK không chứa API key NuGet nào.

```bash
dotnet nuget push artifacts/SofiPOS.FnB.Sdk.1.0.0.nupkg --source https://api.nuget.org/v3/index.json --api-key <NUGET_API_KEY>
```

Hoặc đẩy lên feed riêng (Azure Artifacts, GitHub Packages, thư mục mạng) bằng `--source` tương ứng.

## Hỗ trợ

Bản quyền © SofiPOS. Khi cần hỗ trợ, gửi kèm `TraceId` trong `SofiPosApiException` để SofiPOS tra cứu nhanh.
