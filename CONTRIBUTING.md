# Đóng góp cho SofiPOS SDKs

Cảm ơn bạn đã giúp cải thiện trải nghiệm tích hợp SofiPOS.

## Báo lỗi và đề xuất

Tạo GitHub Issue bằng biểu mẫu tương ứng. Với lỗi, cung cấp version SDK, môi trường, bước tái hiện, kết quả mong đợi và `traceId` nếu có. Xem [SUPPORT.md](SUPPORT.md) và [SECURITY.md](SECURITY.md) trước khi gửi log hoặc báo cáo bảo mật.

## Thay đổi mã nguồn

1. Fork repository và tạo nhánh từ `main`, ví dụ `fix/fnb-order-note` hoặc `docs/webhook-guide`.
2. Giữ thay đổi tập trung vào một vấn đề; theo cấu trúc và cách đặt tên hiện có.
3. Giữ nguyên tiếng Việt, dấu và Unicode; lưu file UTF-8 không BOM.
4. Cập nhật tài liệu hoặc `CHANGELOG.md` khi thay đổi ảnh hưởng người dùng SDK.
5. Mở pull request, mô tả vấn đề, hành vi sau thay đổi và những kiểm tra thực tế đã thực hiện.

### Nhánh và commit

Nhánh `main` chứa các thay đổi đã được review. Làm việc trên nhánh riêng:

```powershell
git switch main
git pull --ff-only
git switch -c docs/webhook-guide
```

Dùng commit mô tả mục đích, ví dụ `fix(fnb): preserve branch when updating order` hoặc `docs: clarify webhook verification`. Các nhóm `feat`, `fix`, `docs`, `chore` giúp đọc lịch sử; ghi rõ thay đổi không tương thích trong mô tả pull request và changelog.

Nếu workspace đang có thay đổi, lưu chúng vào commit của nhánh đang làm trước khi chuyển sang công việc khác. Giữ phần thay đổi nghiệp vụ và phần tài liệu/định dạng thành các commit dễ review khi có thể.

SDK mới đặt tại `sdks/{model}/{language}`; ứng dụng demo đặt tại `examples/{model}/`, với một `frontend/` dùng chung và `backends/{language}/` cho từng SDK. Backend FnB mới tuân theo [hợp đồng HTTP](examples/fnb/CONTRACT.md); không sao chép frontend theo ngôn ngữ SDK. Không thêm thư mục SDK rỗng để biểu thị chức năng chưa triển khai.

Không đưa Partner Key, webhook secret, connection string có mật khẩu, keyring hoặc dữ liệu khách hàng vào commit. Chỉ dùng cấu hình mẫu đã bỏ thông tin xác thực.

## Kiểm tra phù hợp với thay đổi

Từ thư mục gốc, kiểm tra encoding, JSON, đường dẫn tài liệu và tham chiếu project:

```powershell
node scripts/check-source.mjs
git diff --check
```

Lệnh kiểm tra source cần Node.js 22 trở lên, không cài dependency, build hoặc gọi POS.

Với frontend, có thể kiểm tra TypeScript mà không tạo bundle:

```powershell
Set-Location examples/fnb/frontend
npm ci --ignore-scripts
npm run check
```

Repository có bộ test SDK tại `sdks/fnb/dotnet/SofiPOS.FnB.Sdk.Tests`. Việc chạy test, build đầy đủ, tạo gói hoặc gọi POS thật được thực hiện khi phạm vi thay đổi và yêu cầu review cần đến; nêu rõ trong pull request. Không khẳng định đã kiểm tra khi chưa chạy.

## Tương thích

Giữ namespace, tên API public và hành vi hiện có khi sửa lỗi. Nếu đề xuất thay đổi không tương thích, mô tả tác động và cách chuyển đổi trước khi triển khai.

Ghi thay đổi chưa phát hành vào `Unreleased`. Cập nhật [hướng dẫn chuyển đổi](docs/migration-guide.md) khi thay chữ ký hàm, model hoặc cấu hình tích hợp; quy trình version/tag nằm trong [hướng dẫn phát hành](docs/maintainers/releases.md).

Các đóng góp được cung cấp theo [MIT License](LICENSE) của repository.
