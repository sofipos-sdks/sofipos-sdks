# Bắt đầu với SofiPOS FnB SDK

[Trang chính](../README.md) · [Hướng dẫn tích hợp](integration-guide.md) · [SDK .NET](../sdks/fnb/dotnet/README.md)

## Chuẩn bị

| Mục đích | Yêu cầu |
|---|---|
| Tích hợp SDK | Ứng dụng .NET 8 trở lên và .NET SDK phù hợp |
| Chạy ví dụ console có sẵn | .NET SDK 8 trở lên |
| Chạy SDK Lab | .NET SDK 10, Node.js 22.12 trở lên và MongoDB |
| Gọi Partner API | API Base URL và Partner Key do cửa hàng cấp |

SDK không cần MongoDB. MongoDB chỉ dùng để lưu dữ liệu của ứng dụng SDK Lab.

## 1. Lấy mã nguồn

```powershell
git clone https://github.com/sofipos-sdks/sofipos-sdks.git
Set-Location sofipos-sdks
```

Mở solution tại `sdks/fnb/dotnet/SofiPOS.FnB.sln` nếu dùng Visual Studio.

## 2. Tham chiếu SDK

Trong ứng dụng .NET của bạn, thêm reference đến project SDK trong bản clone:

```powershell
dotnet add path/to/YourApp.csproj reference path/to/sofipos-sdks/sdks/fnb/dotnet/SofiPOS.FnB.Sdk/SofiPOS.FnB.Sdk.csproj
```

Hai đường dẫn `path/to/...` cần thay bằng đường dẫn thực tế trên máy của bạn. Đây là cách dùng được ngay, không cần có package trên NuGet.org.

Sau khi SofiPOS công bố package trên NuGet.org, có thể cài bằng:

```powershell
dotnet add package SofiPOS.FnB.Sdk --version 1.0.0
```

Chỉ dùng version đã được công bố trong bản phát hành. File `.nupkg` do SofiPOS cung cấp cũng có thể cài từ một NuGet feed local theo [README SDK](../sdks/fnb/dotnet/README.md).

## 3. Cấu hình kết nối

Partner Key được lấy từ **Hệ thống > Kết nối bên thứ 3** trên SofiPOS. Mỗi key gắn với một cửa hàng.

Trong PowerShell, đặt biến môi trường cho tiến trình đang chạy:

```powershell
$env:SOFIPOS_BASE_URL = 'https://api.example.com'
$env:SOFIPOS_API_KEY = Read-Host 'Nhập Partner Key do cửa hàng cấp'
```

Thay URL mẫu bằng địa chỉ thực tế do SofiPOS cung cấp. Trong ứng dụng vận hành, đọc key từ cấu hình hoặc kho bí mật của bạn.

## 4. Gọi API đầu tiên

```csharp
using SofiPOS.FnB;
using SofiPOS.FnB.Configuration;

using var client = new SofiPosFnBClient(new SofiPosOptions
{
    BaseUrl = Environment.GetEnvironmentVariable("SOFIPOS_BASE_URL")
        ?? throw new InvalidOperationException("Thiếu SOFIPOS_BASE_URL."),
    ApiKey = Environment.GetEnvironmentVariable("SOFIPOS_API_KEY")
        ?? throw new InvalidOperationException("Thiếu SOFIPOS_API_KEY.")
});

var branches = await client.Lookups.GetBranchesAsync();
foreach (var branch in branches)
    Console.WriteLine($"{branch.Id}: {branch.Name}");
```

Tạo client một lần rồi tái sử dụng. Nếu tích hợp ASP.NET Core hoặc nhiều cửa hàng, xem phần DI và `ISofiPosFnBClientFactory` trong [README SDK](../sdks/fnb/dotnet/README.md).

## Chạy ví dụ hoặc giao diện demo

Từ thư mục gốc repository, ví dụ console mặc định chỉ đọc chi nhánh, khu vực/bàn, menu và đơn gần đây:

```powershell
dotnet run --project sdks/fnb/dotnet/examples/SofiPOS.FnB.Example
```

Muốn thao tác bằng form và theo dõi request/response, làm theo [hướng dẫn SDK Lab](../examples/fnb-dotnet-react/README.md). Các thao tác ghi của demo tác động đến cửa hàng đã kết nối; dùng cửa hàng dành cho tích hợp khi thử tạo đơn và thanh toán.

## Khi gặp lỗi

| Tình huống | Kiểm tra |
|---|---|
| `401` | Partner Key, cửa hàng và trạng thái kết nối bên thứ 3 |
| `404` hoặc `422` | ID, chi nhánh, quyền thao tác và điều kiện nghiệp vụ |
| Timeout hoặc lỗi mạng | Base URL, kết nối mạng, chứng chỉ TLS |
| Demo trả `503` | MongoDB và cấu hình `Mongo:ConnectionString` |

Giữ `traceId` khi liên hệ hỗ trợ. Xem [SUPPORT.md](../SUPPORT.md) để biết thông tin cần gửi.
