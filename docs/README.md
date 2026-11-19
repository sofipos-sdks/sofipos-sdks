# Tài liệu SofiPOS SDKs

[Trang chính](../README.md) · [SDK .NET](../sdks/fnb/dotnet/README.md) · [SDK Lab](../examples/fnb/README.md)

## Dành cho đối tác tích hợp

| Tài liệu | Khi nào cần đọc |
|---|---|
| [Bắt đầu nhanh](getting-started.md) | Cài SDK từ source hoặc package và thực hiện lời gọi đầu tiên |
| [Hướng dẫn tích hợp](integration-guide.md) | Kết nối cửa hàng, chi nhánh, phân trang, đơn hàng, thanh toán và webhook |
| [SDK FnB .NET](../sdks/fnb/dotnet/README.md) | Cấu hình, DI, client factory, model và nhóm phương thức |
| [Hướng dẫn chuyển đổi](migration-guide.md) | Cập nhật ứng dụng đã dùng chữ ký API hoặc cấu trúc demo cũ |
| [SDK Lab](../examples/fnb/README.md) | Thử API bằng form, quan sát request/response và nhận webhook |
| [Độ phủ chức năng](../examples/fnb/COVERAGE.md) | Tra tên phương thức SDK, endpoint và form tương ứng |
| [Changelog](../CHANGELOG.md) | Theo dõi thay đổi và khả năng tương thích |

SDK gọi `/api/partner/v1` bằng Partner Key của cửa hàng. API Base URL, key và quyền tích hợp do cửa hàng SofiPOS cấp riêng.

## Dành cho người đóng góp và maintainer

| Tài liệu | Nội dung |
|---|---|
| [Đóng góp](../CONTRIBUTING.md) | Quy ước source, nhánh, kiểm tra và pull request |
| [Hợp đồng HTTP của SDK Lab](../examples/fnb/CONTRACT.md) | Thêm backend cho ngôn ngữ SDK khác và giữ frontend dùng chung |
| [Quản trị GitHub](maintainers/github.md) | Organization, repository và trang giới thiệu |
| [Phát hành SDK](maintainers/releases.md) | Version, tag, metadata NuGet và công bố bản phát hành |
| [Hỗ trợ](../SUPPORT.md) | Báo lỗi SDK, câu hỏi tích hợp và đầu mối vận hành |
| [Bảo mật](../SECURITY.md) | Báo cáo riêng và xử lý thông tin xác thực |

README của SDK là tài liệu sử dụng thư viện. Hợp đồng HTTP của SDK Lab chỉ mô tả API của ứng dụng demo; các hệ thống tích hợp gọi Partner API qua SDK tương ứng.
