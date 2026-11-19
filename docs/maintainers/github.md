# Quản trị GitHub và phát hành SDK

[Trang chính](../../README.md) · [Đóng góp](../../CONTRIBUTING.md) · [Changelog](../../CHANGELOG.md)

## Đích public

| Thuộc tính | Giá trị |
|---|---|
| Organization | `sofipos-sdks` |
| Tài khoản owner | `Toanf2103` |
| Repository | `sofipos-sdks` |
| Nhánh mặc định | `main` |
| Visibility | Public |
| License | MIT |
| Description | SDKs, integration guides and examples for SofiPOS partners. |
| Topics gợi ý | `sofipos`, `pos`, `fnb`, `sdk`, `dotnet`, `partner-api`, `webhooks` |

Giữ một repository cho SDK và ví dụ trong giai đoạn hiện tại. Mỗi SDK vẫn có version và tag riêng. Khi có nhiều SDK với chu kỳ phát hành độc lập, có thể tách repository và chuyển tài liệu/đường dẫn package tương ứng.

## Quy trình thay đổi

Repository đã public. Phát triển trên nhánh riêng, tạo pull request và review trước khi merge vào `main`.

```powershell
git switch main
git pull --ff-only
git switch -c docs/integration-guide
node scripts/check-source.mjs
git diff --check
```

Sau khi hoàn thành thay đổi, commit các file thuộc phạm vi công việc và đẩy nhánh để mở pull request. Xem [CONTRIBUTING.md](../../CONTRIBUTING.md) cho quy ước commit và thông tin review.

Chỉ đưa cấu hình mẫu vào Git. `.env`, `appsettings.Development.json`, `.local-data`, thư mục `net` cũ, `bin`, `obj`, `node_modules` và package build được bỏ qua. Xử lý thông tin xác thực đã lộ theo [SECURITY.md](../../SECURITY.md).

## Cấu hình repository trên GitHub

- Điền description và topics; ghim repository trên profile tài khoản.
- Bật Issues và **Private vulnerability reporting**; tắt Wiki nếu không dùng để tránh phân tán tài liệu.
- Với ruleset cho `main`, yêu cầu pull request và các status check `Repository source`, `Frontend TypeScript` sau khi workflow đã chạy thành công. Cấu hình người review và quyền bypass phù hợp với số maintainer thực tế.
- Bật secret scanning/push protection khi tài khoản và repository hỗ trợ.
- Chỉ cấp quyền ghi cho người phát hành và maintainer cần thiết.

Workflow [Source checks](../../.github/workflows/source-checks.yml) kiểm tra UTF-8, JSON, liên kết nội bộ, tham chiếu project và TypeScript; không build, chạy test hay publish. Dependabot đề xuất cập nhật NuGet, npm và GitHub Actions hằng tuần. Source checks hiện chưa xác minh SDK biên dịch hoặc hành vi API.

Tham khảo chính thức: [rulesets](https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-rulesets/about-rulesets), [private vulnerability reporting](https://docs.github.com/en/code-security/how-tos/report-and-fix-vulnerabilities/configure-vulnerability-reporting/configure-for-a-repository).

## Trang tổ chức tương tự Grab

Trang [Grab](https://github.com/grab) là profile GitHub Organization với danh sách repository được ghim. Organization của SofiPOS là `sofipos-sdks`, do tài khoản `Toanf2103` quản lý. File cấu hình local không tự tạo organization trên GitHub.

Khi organization đã được tạo:

1. Đặt thông tin thương hiệu, website và kênh hỗ trợ đã xác nhận trong profile organization.
2. Tạo repository public tên `.github` trong organization đó.
3. Dùng nội dung từ [organization-profile.md](organization-profile.md) cho `profile/README.md` của repository `.github`.
4. Ghim repository SDK và tài liệu có thật; thêm repository mới khi có sản phẩm tương ứng.
5. Nếu chuyển repository SDK sang organization, cập nhật URL trong README, metadata NuGet và tài liệu.

Đặt `.github/profile/README.md` trong repository SDK này không tạo trang giới thiệu organization. GitHub yêu cầu file `profile/README.md` nằm trong repository riêng có tên `.github`.

Xem [hướng dẫn profile organization của GitHub](https://docs.github.com/en/organizations/collaborating-with-groups-in-organizations/customizing-your-organizations-profile).

## Phiên bản và phát hành

Version nằm trong `SofiPOS.FnB.Sdk.csproj`; tag theo SDK có dạng `fnb-dotnet-vMAJOR.MINOR.PATCH`. Quy trình chọn version, đọc metadata, đóng gói, tạo GitHub Release và phát hành NuGet nằm trong [releases.md](releases.md).

Nội dung chưa phát hành nằm trong `Unreleased` của [CHANGELOG.md](../../CHANGELOG.md). Các thay đổi không tương thích cần cập nhật [hướng dẫn chuyển đổi](../migration-guide.md).
