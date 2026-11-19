# Changelog

Các thay đổi ảnh hưởng đến đối tác tích hợp được ghi tại đây. Version gói SDK tuân theo `MAJOR.MINOR.PATCH`; tag theo SDK, ví dụ `fnb-dotnet-v1.0.0`.

## Unreleased

### Trải nghiệm tích hợp và phát hành SDK

- README có bảng chọn SDK, lối vào quickstart/demo và liên kết tài liệu theo nhu cầu tích hợp.
- Thêm mục lục tài liệu, hướng dẫn chuyển đổi API chi nhánh/nhóm sản phẩm và quy trình phát hành NuGet theo version/tag.
- Kiểm tra source bằng Node.js không cần dependency: UTF-8 không BOM, JSON, liên kết tài liệu, `ProjectReference` và project trong solution. CI chạy kiểm tra này cùng kiểm tra TypeScript.
- Bổ sung release notes trong metadata package và `ContinuousIntegrationBuild` cho môi trường GitHub Actions; chuẩn hóa quy tắc xuống dòng cho các file source và cấu hình.

### Ứng dụng thử SDK

- Nhập thông tin kết nối, xác minh bằng SDK và gọi API trực tiếp bằng form; các tham chiếu chọn theo tên từ API.
- Kết nối, tối đa 200 HTTP request và 200 webhook chỉ giữ trong RAM; bỏ phụ thuộc MongoDB, snapshot và worker đồng bộ.
- Giữ trace HTTP, xác minh/mô phỏng webhook; tăng cỡ chữ giao diện 30% và tắt mã màu ANSI trong log FE.

### Cấu trúc SDK Lab

- Demo FnB chuyển sang `examples/fnb`, gồm một frontend React dùng chung và backend theo ngôn ngữ tại `backends/{language}`; backend hiện có là `dotnet`.
- Frontend chọn backend qua `BACKEND_URL`; tài liệu `examples/fnb/CONTRACT.md` quy định HTTP dùng chung để bổ sung backend Node.js hoặc ngôn ngữ khác.

### Thay đổi không tương thích ngược: chi nhánh và nhóm sản phẩm theo từng request

- Đối tác không còn chi nhánh mặc định và nhóm sản phẩm mặc định trong cấu hình kết nối.
- Mọi hàm `Fnb`, `FnbOrders`, `FnbPayments` nhận `branchId` làm tham số đầu và gửi qua query `branchId`, kể cả đọc/sửa đơn, thanh toán và kiểm tra giao dịch QR. `FnbOrderListQuery.BranchId` chuyển sang bắt buộc (`Guid`). Đơn phải thuộc chi nhánh truyền vào.
- `ProductCreateRequest.CategoryId` bắt buộc; thêm `Lookups.GetProductCategoriesAsync()` và model `ProductCategory`; `Product.CategoryId` trả nhóm hiện tại.
- SDK Lab: bỏ `DefaultBranchId` khỏi cấu hình kết nối, chọn chi nhánh ở thanh trên cùng hoặc trong form; form tạo sản phẩm có chọn nhóm; webhook đơn đọc lại theo `data.branchId`.

### Chuẩn bị phát hành public

- SDK FnB cho .NET 8 trở lên, version trong source là `1.0.0`.
- 13 nhóm chức năng Partner API, phân trang, retry GET, lỗi có `traceId` và xác minh webhook.
- SDK Lab dùng .NET 10 và React, không cần database; tài liệu đối chiếu 92 thao tác.
- Tài liệu bắt đầu tích hợp, hỗ trợ, đóng góp và bảo mật.
- Giấy phép MIT và cấu hình repository public.

Chưa xác nhận phát hành NuGet.org. Gói build cục bộ không được xem là bản phát hành public.
