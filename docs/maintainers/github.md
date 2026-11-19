# Quản trị GitHub và phát hành SDK

[Trang chính](../../README.md) · [Đóng góp](../../CONTRIBUTING.md) · [Changelog](../../CHANGELOG.md)

## Đích public

| Thuộc tính | Giá trị |
|---|---|
| Organization dự kiến | `sofipos-sdks` |
| Tài khoản owner | `Toanf2103` |
| Repository | `sofipos-sdks` |
| Nhánh mặc định | `main` |
| Visibility | Public |
| License | MIT |
| Description | SDKs, integration guides and examples for SofiPOS partners. |
| Topics gợi ý | `sofipos`, `pos`, `fnb`, `sdk`, `dotnet`, `partner-api`, `webhooks` |

Giữ một repository cho SDK và ví dụ trong giai đoạn hiện tại. Mỗi SDK vẫn có version và tag riêng. Khi có nhiều SDK với chu kỳ phát hành độc lập, có thể tách repository và chuyển tài liệu/đường dẫn package tương ứng.

## Lần public đầu tiên

1. Kiểm tra chỉ có cấu hình mẫu trong danh sách file Git. `.env`, `appsettings.Development.json`, `.local-data`, thư mục `net` cũ, `bin`, `obj`, `node_modules` và gói build phải được bỏ qua.
2. Đổi thông tin xác thực đã từng bị lộ trước khi chia sẻ source. Xem [SECURITY.md](../../SECURITY.md).
3. Tạo repository public trống `sofipos-sdks/sofipos-sdks`, không khởi tạo README, LICENSE hoặc `.gitignore` trên GitHub vì source đã có các file này.
4. Sau khi review source, tạo commit đầu tiên và đẩy nhánh `main`.

```powershell
git add .
git diff --cached --stat
git diff --cached --check
git commit -m "chore: prepare SofiPOS SDKs for public integration"
git remote add origin https://github.com/sofipos-sdks/sofipos-sdks.git
git push -u origin main
```

Nếu `origin` đã có, kiểm tra `git remote -v` và dùng remote đúng; không thêm lần hai. Các lệnh trên dành cho maintainer thực hiện khi sẵn sàng public, không phải tác vụ đã được chạy chỉ vì tài liệu tồn tại.

## Cấu hình repository trên GitHub

- Điền description và topics; ghim repository trên profile tài khoản.
- Bật Issues và **Private vulnerability reporting**; tắt Wiki nếu không dùng để tránh phân tán tài liệu.
- Với ruleset cho `main`, yêu cầu pull request và status check `Frontend TypeScript` sau khi workflow đã chạy thành công. Cấu hình người review và quyền bypass phù hợp với số maintainer thực tế.
- Bật secret scanning/push protection khi tài khoản và repository hỗ trợ.
- Chỉ cấp quyền ghi cho người phát hành và maintainer cần thiết.

Workflow [Source checks](../../.github/workflows/source-checks.yml) chỉ cài dependency frontend và kiểm tra TypeScript; không build, chạy test hay publish. Dependabot đề xuất cập nhật NuGet, npm và GitHub Actions hằng tuần. Source checks hiện chưa xác minh SDK biên dịch hoặc hành vi API.

Tham khảo chính thức: [rulesets](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/about-rulesets), [private vulnerability reporting](https://docs.github.com/en/code-security/how-tos/report-and-fix-vulnerabilities/configure-vulnerability-reporting/configure-for-a-repository).

## Trang tổ chức tương tự Grab

Trang [Grab](https://github.com/grab) là profile GitHub Organization với danh sách repository được ghim. Organization dự kiến của SofiPOS là `sofipos-sdks`, do tài khoản `Toanf2103` quản lý. File cấu hình local không tự tạo organization trên GitHub.

Khi organization đã được tạo:

1. Đặt thông tin thương hiệu, website và kênh hỗ trợ đã xác nhận trong profile organization.
2. Tạo repository public tên `.github` trong organization đó.
3. Dùng nội dung từ [organization-profile.md](organization-profile.md) cho `profile/README.md` của repository `.github`.
4. Ghim repository SDK và tài liệu có thật; thêm repository mới khi có sản phẩm tương ứng.
5. Nếu chuyển repository SDK sang organization, cập nhật URL trong README, metadata NuGet và tài liệu.

Đặt `.github/profile/README.md` trong repository SDK này không tạo trang giới thiệu organization. GitHub yêu cầu file `profile/README.md` nằm trong repository riêng có tên `.github`.

Xem [hướng dẫn profile organization của GitHub](https://docs.github.com/en/organizations/collaborating-with-groups-in-organizations/customizing-your-organizations-profile).

## Phiên bản và phát hành

Version nằm trong `SofiPOS.FnB.Sdk.csproj`. Dùng tag `fnb-dotnet-vMAJOR.MINOR.PATCH`, ví dụ `fnb-dotnet-v1.0.0`; thêm release notes mô tả thay đổi, tương thích và các kiểm tra đã thực hiện. Chỉ chuyển nội dung `Unreleased` thành version có ngày phát hành sau khi thực sự phát hành.

Gói `artifacts` có sẵn trên máy trước khi chuẩn bị public có thể chưa chứa metadata repository/MIT mới. Tạo lại từ commit/tag được chọn trước khi phân phối. Các lệnh đóng gói dưới đây chỉ chạy theo quyết định phát hành của maintainer:

```powershell
dotnet pack sdks/fnb/dotnet/SofiPOS.FnB.Sdk/SofiPOS.FnB.Sdk.csproj -c Release -o sdks/fnb/dotnet/artifacts
```

Trước khi phát hành, thực hiện kiểm tra SDK phù hợp với thay đổi và ghi rõ kết quả. Workflow hiện có không thay thế kiểm tra .NET. Khi phát hành `1.0.0`, xác nhận manifest của package chứa version `1.0.0`, license `MIT`, URL repository và README đúng; mở gói để kiểm tra nội dung.

Tạo GitHub Release từ tag đã chọn, đính kèm `.nupkg` và `.snupkg` mới. Nếu phát hành NuGet.org, người phát hành cần quyền sở hữu package và thông tin xác thực NuGet riêng. Chỉ bổ sung workflow publish khi đã cấu hình quyền phát hành và phê duyệt cần thiết.

Sau khi package thực sự có trên NuGet.org, xác nhận khả năng cài trong một ứng dụng riêng rồi cập nhật hướng dẫn cài đặt. Không gắn badge thành công, badge NuGet hoặc tuyên bố đã phát hành trước khi kiểm tra trạng thái thực tế.
