# Backend .NET của FnB SDK Lab

ASP.NET Core .NET 10 gọi trực tiếp SDK tại `sdks/fnb/dotnet` và phục vụ [frontend dùng chung](../../frontend). Cấu hình kết nối, trace và webhook chỉ nằm trong RAM; không cần MongoDB hoặc keyring.

Từ thư mục gốc repository:

```powershell
Push-Location examples/fnb/backends/dotnet
dotnet restore
dotnet run
Pop-Location
```

Mặc định `http://localhost:5080`, cấu hình cổng trong `appsettings.json`/launch profile. Nhập API Base URL, Partner Key và secret tùy chọn qua giao diện. Không lưu các thông tin này vào file; khởi động lại backend phải kết nối lại.

- `Controllers/`: mỗi thao tác gọi SDK tường minh; các endpoint cấu hình, trace và webhook.
- `Services/ConnectionService.cs`: kiểm tra bằng SDK và giữ kết nối trong RAM.
- `Services/SdkGateway.cs`, `SdkTraceHandler.cs`: gọi SDK, trả kết quả và ghi HTTP thực tế đã che credential.
- `DemoStore.cs`: tối đa 200 HTTP request và 200 webhook trong RAM.
- `Services/WorkspaceSession.cs`: điều phối một phiên kết nối, tránh request cũ dùng credential mới.

Không có database, snapshot, migration hay worker đồng bộ. File/database/keyring từ demo cũ không bị xóa và không dùng làm nơi lưu phiên. Các backend khác tuân theo [CONTRACT.md](../../CONTRACT.md). Hướng dẫn giao diện và webhook ở [README chung](../../README.md).
