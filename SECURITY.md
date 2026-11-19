# Chính sách bảo mật

## Báo cáo riêng

Không đăng lỗ hổng, thông tin xác thực hoặc dữ liệu cửa hàng lên Issue public.

Nếu repository đã bật **Private vulnerability reporting**, mở tab **Security → Advisories → Report a vulnerability** để gửi riêng cho maintainer. Nếu chưa thấy chức năng này, liên hệ riêng đầu mối tích hợp SofiPOS đang làm việc với bạn.

Trong báo cáo, cung cấp commit/version bị ảnh hưởng, điều kiện tái hiện, tác động và ví dụ tối thiểu đã loại bỏ dữ liệu thật. Không cần khai thác trên cửa hàng thực tế để chứng minh lỗi.

## Thông tin xác thực bị lộ

Nếu Partner Key, webhook secret hoặc mật khẩu dịch vụ đã bị chia sẻ, thu hồi/đổi thông tin đó qua cửa hàng hoặc dịch vụ tương ứng. Xóa khỏi source không làm thông tin xác thực cũ mất hiệu lực.

## Phiên bản

Mã nguồn đã public; các thay đổi chưa gắn bản phát hành nằm trong mục `Unreleased` của [CHANGELOG.md](CHANGELOG.md). Chưa có cam kết duy trì các nhánh phiên bản cũ; khi có bản phát hành, thông tin sửa lỗi và tương thích được ghi trong changelog và release notes.

## Demo

SDK Lab phục vụ phát triển tích hợp. Các endpoint quản lý hiện chưa có đăng nhập/phân quyền người dùng. Hướng dẫn vận hành demo và giới hạn công khai callback nằm trong [README demo](examples/fnb/README.md).
