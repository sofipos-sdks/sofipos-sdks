// Ví dụ dùng SofiPOS FnB SDK.
//
// Chạy thử kiểm tra chữ ký webhook (không cần mạng):
//   dotnet run -- webhook
//
// Gọi API thật (chỉ đọc dữ liệu):
//   SOFIPOS_BASE_URL=https://<domain-api> SOFIPOS_API_KEY=<partner-key> dotnet run
//
// Tạo một đơn thử (GHI dữ liệu vào cửa hàng, chỉ dùng với cửa hàng thử nghiệm):
//   dotnet run -- create-order
//
// Partner Key chỉ đọc từ biến môi trường, không ghi trong mã nguồn và không in ra màn hình.

using System.Text;
using SofiPOS.FnB;
using SofiPOS.FnB.Configuration;
using SofiPOS.FnB.Exceptions;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Webhooks;

Console.OutputEncoding = Encoding.UTF8;
var mode = args.FirstOrDefault()?.ToLowerInvariant() ?? "read";

if (mode == "webhook")
{
    RunWebhookDemo();
    return 0;
}

var baseUrl = Environment.GetEnvironmentVariable("SOFIPOS_BASE_URL");
var apiKey = Environment.GetEnvironmentVariable("SOFIPOS_API_KEY");
if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("Cần đặt biến môi trường SOFIPOS_BASE_URL và SOFIPOS_API_KEY trước khi gọi API.");
    Console.WriteLine("Chạy 'dotnet run -- webhook' để thử phần kiểm tra chữ ký webhook mà không cần mạng.");
    return 1;
}

var client = new SofiPosFnBClient(new SofiPosOptions
{
    BaseUrl = baseUrl,
    ApiKey = apiKey,
    Timeout = TimeSpan.FromSeconds(30)
});

try
{
    var branches = await client.Lookups.GetBranchesAsync();
    Console.WriteLine($"Cửa hàng có {branches.Count} chi nhánh:");
    foreach (var branch in branches) Console.WriteLine($"  - {branch.Name} ({branch.Id}){(branch.IsActive ? "" : " [ngừng]")}");

    // Chi nhánh truyền rõ ở mọi API FnB (query branchId); ví dụ chọn chi nhánh đang hoạt động đầu tiên.
    var activeBranch = branches.FirstOrDefault(b => b.IsActive);
    if (activeBranch == null)
    {
        Console.WriteLine("Không có chi nhánh đang hoạt động.");
        return 0;
    }
    var branchId = activeBranch.Id;

    var areas = await client.Fnb.GetAreasAsync(branchId);
    Console.WriteLine($"\nKhu vực / bàn ({areas.Count} khu vực):");
    foreach (var area in areas) Console.WriteLine($"  - {area.Name}: {area.Tables?.Count ?? 0} bàn");

    var menu = await client.Fnb.GetMenuAsync(branchId, "TaiCho");
    Console.WriteLine($"\nThực đơn kênh {menu.SaleChannelId} (giá tính lúc {menu.PricedAt:HH:mm dd/MM/yyyy}): {menu.Products.Count} món");
    foreach (var product in menu.Products.Take(10))
        Console.WriteLine($"  - {product.Code} {product.Name}: {product.PriceIncludeVAT:N0}đ/{product.UnitName}");

    var orders = await client.FnbOrders.ListAsync(new FnbOrderListQuery { BranchId = branchId, PageSize = 5 });
    Console.WriteLine($"\nĐơn gần đây: {orders.MetaData.TotalCount} đơn");
    foreach (var order in orders.Items) Console.WriteLine($"  - {order.Code} {order.CreateDate:HH:mm dd/MM} {order.TotalPrice:N0}đ");

    if (mode == "create-order")
    {
        var first = menu.Products.FirstOrDefault()
            ?? throw new InvalidOperationException("Thực đơn không có món để tạo đơn thử.");

        // RequestId giữ nguyên khi gửi lại cùng một đơn để SofiPOS không tạo đơn trùng.
        var created = await client.FnbOrders.CreateAsync(branchId, new FnbOrderCreateRequest
        {
            RequestId = Guid.NewGuid(),
            SaleChannelId = "TaiCho",
            Note = "Đơn thử từ SofiPOS FnB SDK",
            Items = { new FnbOrderItemRequest { ProductId = first.Id, Quantity = 1 } }
        });
        Console.WriteLine($"\nĐã tạo đơn {created.Code} ({created.Id}).");

        var preview = await client.FnbPayments.PreviewAsync(branchId, created.Id);
        Console.WriteLine($"Tạm tính cần thanh toán: {preview.TotalAmount:N0}đ");
    }

    return 0;
}
catch (SofiPosApiException ex)
{
    Console.WriteLine($"Lỗi API {(int)ex.StatusCode}: {ex.Message} (trace-id: {ex.TraceId ?? "-"})");
    foreach (var error in ex.Errors) Console.WriteLine($"  - {error.Field}: {error.Message}");
    return 2;
}
catch (TimeoutException ex)
{
    Console.WriteLine(ex.Message);
    return 3;
}

static void RunWebhookDemo()
{
    // Secret thật lấy từ màn hình cấu hình đối tác của cửa hàng (whsec_...). Đây chỉ là giá trị thử.
    const string secret = "whsec_demo_only";
    const string body = "{\"id\":\"evt_demo\",\"event\":\"order.created\",\"createdAt\":\"2026-11-18T15:30:00+07:00\","
        + "\"data\":{\"orderId\":\"0b6f2a9e-1c3d-4e5f-8a7b-9c0d1e2f3a4b\",\"code\":\"DH000123\",\"branchId\":\"6f1c2a1e-1d5b-4d0e-9a0a-2b1f6c1e9a11\"}}";
    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    // Phía SofiPOS ký như sau; phía đối tác chỉ cần gọi VerifyAndParse.
    var signature = SofiPosWebhookVerifier.ComputeSignature(secret, timestamp, body);

    var webhookEvent = SofiPosWebhookVerifier.VerifyAndParse(secret, timestamp.ToString(), signature, body);
    Console.WriteLine($"Chữ ký hợp lệ. Sự kiện {webhookEvent.Event} ({webhookEvent.Id})");
    if (webhookEvent.IsOrderEvent)
    {
        var data = webhookEvent.GetOrderData();
        Console.WriteLine($"  Đơn {data.Code} ({data.OrderId}) tại chi nhánh {data.BranchId}");
        Console.WriteLine("  Bước tiếp theo: gọi client.FnbOrders.GetAsync(data.BranchId, orderId) để lấy dữ liệu mới nhất.");
    }

    var tampered = body.Replace("DH000123", "DH999999");
    Console.WriteLine($"Body bị sửa thì IsValid = {SofiPosWebhookVerifier.IsValid(secret, timestamp.ToString(), signature, tampered)}");
}
