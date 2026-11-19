# Phát hành SofiPOS FnB SDK cho .NET

[Trang chính](../../README.md) · [Quản trị GitHub](github.md) · [Changelog](../../CHANGELOG.md)

## Trạng thái và phạm vi

Version khai báo trong `SofiPOS.FnB.Sdk.csproj` hiện là `1.0.0`. Source trên GitHub, file `.nupkg` build local, GitHub Release và package NuGet.org là các trạng thái riêng. Chỉ ghi một version là đã phát hành sau khi kiểm tra nơi phân phối tương ứng.

SDK target .NET 8; SDK Lab dùng .NET 10 và có chu kỳ thay đổi riêng. Phát hành NuGet chỉ đóng gói thư viện `SofiPOS.FnB.Sdk`, không đóng gói demo hoặc project test.

## 1. Chọn version và chốt nội dung

| Thành phần version | Khi thay đổi |
|---|---|
| `MAJOR` | Có thay đổi không tương thích với API public của bản đã phát hành |
| `MINOR` | Thêm chức năng có tương thích ngược |
| `PATCH` | Sửa lỗi có tương thích ngược |

Dùng version preview như `1.0.0-preview.1` nếu muốn phân phối để đối tác đánh giá trước bản ổn định. Không thay nội dung một version đã công bố; sửa đổi tiếp theo cần version mới.

Cập nhật `Version` trong project, chuyển các mục phù hợp từ `Unreleased` trong `CHANGELOG.md` sang version/ngày phát hành thực tế, và cập nhật hướng dẫn chuyển đổi nếu có API thay đổi. Ví dụ đổi chữ ký hàm để yêu cầu `branchId` phải được mô tả rõ trong release notes.

## 2. Review và kiểm tra source

Tạo pull request vào `main`, ghi rõ thay đổi, tương thích và những kiểm tra đã thực hiện. Từ thư mục gốc:

```powershell
node scripts/check-source.mjs
git diff --check
dotnet msbuild sdks/fnb/dotnet/SofiPOS.FnB.Sdk/SofiPOS.FnB.Sdk.csproj -getProperty:Version,TargetFramework,PackageLicenseExpression,RepositoryUrl
```

Lệnh `msbuild` ở trên chỉ đọc metadata, không gọi target build. CI `Repository source` và `Frontend TypeScript` kiểm tra source/tài liệu và TypeScript; chưa xác minh SDK biên dịch hoặc hành vi API.

Các bước kiểm tra .NET, test hoặc nghiệm thu gọi POS thật cần được lựa chọn theo thay đổi và yêu cầu phát hành. Ghi kết quả thực tế trong pull request/release notes. Hoàn thành review và merge trước khi chọn commit phát hành.

## 3. Tạo package từ commit phát hành

Các lệnh đóng gói dưới đây bao gồm build. Maintainer chỉ chạy khi đã quyết định tạo bản phát hành, từ source sạch ở commit đã chọn:

```powershell
dotnet pack sdks/fnb/dotnet/SofiPOS.FnB.Sdk/SofiPOS.FnB.Sdk.csproj -c Release -p:ContinuousIntegrationBuild=true -o sdks/fnb/dotnet/artifacts
```

Với version `1.0.0`, kết quả dự kiến:

```text
sdks/fnb/dotnet/artifacts/
├── SofiPOS.FnB.Sdk.1.0.0.nupkg
└── SofiPOS.FnB.Sdk.1.0.0.snupkg
```

Kiểm tra manifest và nội dung gói: đúng version, target framework, dependency, license MIT, README, URL repository và release notes. Dùng file vừa tạo từ commit đã chọn; `artifacts/` được Git bỏ qua.

## 4. Gắn tag và tạo GitHub Release

Mỗi SDK dùng tag riêng, ví dụ `fnb-dotnet-v1.0.0`. Sau khi đã chốt commit phát hành:

```powershell
git tag -a fnb-dotnet-v1.0.0 -m "SofiPOS FnB .NET SDK 1.0.0"
git push origin fnb-dotnet-v1.0.0
```

Tạo GitHub Release từ tag đó và đính kèm `.nupkg`, `.snupkg` tương ứng. Release notes cần nêu:

- Version, commit/tag và phiên bản Partner API tương ứng.
- Chức năng thêm/sửa và tác động tới ứng dụng đối tác.
- Thay đổi không tương thích và liên kết tới hướng dẫn chuyển đổi.
- Yêu cầu .NET, cách cài và những kiểm tra thực tế đã thực hiện.
- Package mới được phát hành ở GitHub Releases, NuGet.org hoặc cả hai.

## 5. Phát hành NuGet.org khi có quyền package

Người phát hành cần quyền sở hữu package trên NuGet.org và thông tin xác thực do NuGet cấp. Đặt key trong môi trường của tiến trình phát hành; ví dụ với PowerShell:

```powershell
dotnet nuget push sdks/fnb/dotnet/artifacts/SofiPOS.FnB.Sdk.1.0.0.nupkg --source https://api.nuget.org/v3/index.json --api-key $env:NUGET_API_KEY
```

Thay đường dẫn bằng version thực tế. Sau khi NuGet xử lý xong, xác nhận version xuất hiện và có thể cài trong ứng dụng riêng trước khi chuyển README sang hướng dẫn cài từ NuGet.org làm mặc định.

Repository hiện không có workflow tự publish. Khi bổ sung workflow, tách bước đóng gói và bước phát hành, dùng commit/tag đã chọn và giới hạn quyền phát hành theo cơ chế xác thực của feed.

## 6. Giữ tài liệu nhất quán

Cập nhật README SDK, quickstart, changelog và trang organization khi có thay đổi ảnh hưởng đối tác. Nội dung nguồn của profile organization ở [organization-profile.md](organization-profile.md); bản hiển thị nằm tại `profile/README.md` của repository `sofipos-sdks/.github`.

Metadata CI sử dụng `ContinuousIntegrationBuild`; Source Link đã có trong .NET SDK 8 cho GitHub. Tham khảo [MSBuild properties](https://learn.microsoft.com/en-us/dotnet/core/project-sdk/msbuild-props#continuousintegrationbuild) và [Source Link trong .NET SDK](https://learn.microsoft.com/en-us/dotnet/core/compatibility/sdk/8.0/source-link).
