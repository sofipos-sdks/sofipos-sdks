# Chuyển đổi sang API FnB hiện tại

[Trang chính](../README.md) · [SDK .NET](../sdks/fnb/dotnet/README.md) · [Changelog](../CHANGELOG.md)

Hướng dẫn này dành cho ứng dụng đã dùng source trước khi chi nhánh được truyền theo từng request. Các thay đổi đang nằm trong `Unreleased`; version trong project vẫn là `1.0.0`, chưa phải thông báo phát hành package mới.

## 1. Truyền chi nhánh cho mọi thao tác FnB

Client tiếp tục gắn với cửa hàng thông qua Partner Key. Với các nhóm `Fnb`, `FnbOrders`, `FnbPayments`, mỗi lời gọi cần xác định chi nhánh của nghiệp vụ.

| Lời gọi trước đây | Lời gọi hiện tại |
|---|---|
| `FnbOrders.GetAsync(orderId)` | `FnbOrders.GetAsync(branchId, orderId)` |
| `FnbOrders.CreateAsync(request, branchId)` | `FnbOrders.CreateAsync(branchId, request)` |
| `FnbPayments.PreviewAsync(orderId, request)` | `FnbPayments.PreviewAsync(branchId, orderId, request)` |
| `FnbPayments.PayAsync(orderId, request)` | `FnbPayments.PayAsync(branchId, orderId, request)` |

Các phương thức sửa dòng món/combo, chuyển bàn, gộp/tách đơn và giao dịch QR cũng nhận `branchId` ở vị trí đầu. `Guid.Empty` bị từ chối trước khi gửi HTTP; SDK gửi chi nhánh qua query `branchId`.

Danh sách đơn dùng query có chi nhánh bắt buộc:

```csharp
var page = await client.FnbOrders.ListAsync(new FnbOrderListQuery
{
    BranchId = branchId,
    PageNumber = 1,
    PageSize = 50
});

var order = await client.FnbOrders.GetAsync(branchId, orderId);
```

Lấy danh sách chi nhánh qua `Lookups.GetBranchesAsync`, sau đó chọn chi nhánh FnB đang hoạt động phù hợp với nghiệp vụ của ứng dụng. Giữ `branchId` cùng định danh đơn trong hệ thống đối tác. Khi gộp hoặc chuyển món, đơn nguồn và đích phải thuộc chi nhánh đã truyền.

## 2. Chọn nhóm khi tạo sản phẩm

`ProductCreateRequest.CategoryId` là bắt buộc. Đọc nhóm qua `Lookups.GetProductCategoriesAsync`, hiển thị để người dùng chọn rồi gửi ID đã chọn:

```csharp
var categories = await client.Lookups.GetProductCategoriesAsync();
// categoryId là ID người dùng chọn từ categories.
var product = await client.Products.CreateAsync(new ProductCreateRequest
{
    ProductCode = "CF001",
    CategoryId = categoryId,
    Name = "Cà phê sữa",
    Price = 30000
});
```

`Product.CategoryId` trả nhóm hiện tại. Model cập nhật sản phẩm chưa có thao tác đổi nhóm. Thêm control chọn nhóm trong ứng dụng nếu form trước đây chỉ gửi mã, tên và giá.

## 3. Dùng chi nhánh trong webhook đơn hàng

Payload đơn có `data.branchId`. Đọc lại đơn bằng cả chi nhánh và ID đơn:

```csharp
var data = webhookEvent.GetOrderData();
var order = await client.FnbOrders.GetAsync(data.BranchId, data.OrderId);
```

Đoạn code trên chạy sau khi đã xác minh chữ ký và nhận diện sự kiện hợp lệ. Khi lưu công việc nền, giữ cả hai ID trong dữ liệu công việc.

## 4. Cập nhật đường dẫn SDK Lab

| Trước đây | Hiện tại |
|---|---|
| `examples/fnb-dotnet-react/frontend` | `examples/fnb/frontend` |
| `examples/fnb-dotnet-react/backend` | `examples/fnb/backends/dotnet` |
| Frontend gắn với backend .NET | Một frontend dùng chung, chọn backend qua `BACKEND_URL` |
| Cấu hình, snapshot và lịch sử trong MongoDB | Kết nối, trace và webhook trong RAM |

Demo không dùng MongoDB, snapshot, worker đồng bộ hoặc keyring. Khi backend khởi động lại, nhập lại kết nối; trace/webhook trong RAM không được giữ qua lần khởi động. Nhận webhook chỉ xác minh và ghi inbox, không tự lấy lại dữ liệu từ POS.

Cập nhật đường dẫn chạy local, cấu hình proxy và script nội bộ của bạn theo [README SDK Lab](../examples/fnb/README.md). Backend cho ngôn ngữ khác tuân theo [hợp đồng HTTP](../examples/fnb/CONTRACT.md).

## 5. Kiểm tra ứng dụng sau khi chuyển đổi

- Tra các lời gọi `FnbOrders` và `FnbPayments`, thêm chi nhánh ở đúng vị trí tham số.
- Gắn `branchId` vào bộ lọc danh sách đơn, dữ liệu công việc nền và bản ghi đơn của hệ thống đối tác.
- Chọn `CategoryId` khi tạo sản phẩm.
- Giữ nguyên `RequestId` khi gửi lại cùng yêu cầu tạo đơn.
- Phân biệt phiên RAM của SDK Lab với dữ liệu do ứng dụng tích hợp của bạn lưu.

SDK Lab và bảng [độ phủ chức năng](../examples/fnb/COVERAGE.md) giúp đối chiếu các thao tác trước khi nghiệm thu trên cửa hàng dành cho tích hợp.
