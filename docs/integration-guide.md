# Hướng dẫn tích hợp Partner API

[Trang chính](../README.md) · [Bắt đầu nhanh](getting-started.md) · [SDK .NET](../sdks/fnb/dotnet/README.md)

## Kết nối và xác thực

`SofiPosOptions.BaseUrl` nhận địa chỉ máy chủ; SDK tự thêm `/api/partner/v1/` nếu chưa có. Partner Key được gửi qua header `X-Partner-Key`.

Mỗi client gắn với một cửa hàng. Khi phục vụ nhiều cửa hàng, tạo client theo key qua `ISofiPosFnBClientFactory`; tránh thay key trên một client đang dùng chung. `ApiKeyProvider` hỗ trợ lấy key từ kho bí mật cho mỗi request.

## Luồng đơn hàng FnB

1. Đọc chi nhánh bằng `Lookups.GetBranchesAsync` và chọn chi nhánh FnB phù hợp.
2. Đọc bàn, menu và phương thức thanh toán qua nhóm `Fnb`.
3. Tạo đơn bằng `FnbOrders.CreateAsync`, giữ `RequestId` cùng đơn trong hệ thống đối tác.
4. Thêm/sửa/hủy món, combo hoặc chuyển/gộp/tách đơn theo nghiệp vụ.
5. Tạm tính bằng `FnbPayments.PreviewAsync`.
6. Nếu dùng chuyển khoản, tạo QR và kiểm tra giao dịch qua `FnbPayments`.
7. Gọi `FnbPayments.PayAsync` để ghi nhận thanh toán.

Tạo QR hoặc nhận kết quả kiểm tra giao dịch không tự chốt đơn. Các model và ví dụ đầy đủ nằm trong [README SDK](../sdks/fnb/dotnet/README.md); [bảng độ phủ](../examples/fnb-dotnet-react/COVERAGE.md) đối chiếu từng phương thức với endpoint.

## Retry và chống trùng

SDK mặc định thử lại tối đa hai lần cho GET khi gặp lỗi mạng, timeout hoặc mã `429`, `502`, `503`, `504`. SDK xử lý `Retry-After`, với thời gian đợi được giới hạn tối đa 30 giây cho mỗi lần retry. `MaxRetries = 0` tắt retry.

Thao tác ghi không tự retry. Nếu tạo đơn bị mất kết nối hoặc timeout, giữ nguyên `RequestId` khi gửi lại cùng yêu cầu. Với thao tác khác, đọc lại dữ liệu để xác định kết quả trước khi quyết định gửi lại.

## Phân trang và thời gian

Các danh sách có `MetaData`; dùng `ListAllAsync` khi cần duyệt tất cả các trang. Cân nhắc phạm vi chi nhánh và bộ lọc khi đồng bộ.

Thời gian nghiệp vụ SofiPOS dùng UTC+7. SDK đọc `DateTime` thành giờ Việt Nam với `DateTimeKind.Unspecified`; giá trị gửi lên có `Kind = Utc` hoặc `Local` được đổi sang UTC+7. `Unspecified` được coi là đã ở UTC+7.

## Xử lý lỗi

Lỗi HTTP được biểu diễn bằng `SofiPosApiException`: `StatusCode`, `ErrorCode`, `Message`, `Errors`, `TraceId` và `ResponseBody`. Có thể dùng các cờ `IsUnauthorized`, `IsNotFound`, `IsConflict`, `IsValidationError`, `IsTransient` để phân nhánh xử lý.

Timeout ném `TimeoutException`; hủy chủ động qua `CancellationToken` ném `OperationCanceledException`. Giữ `traceId` để tra cứu, và che thông tin xác thực/dữ liệu khách hàng khi lưu hoặc chia sẻ log.

## Webhook

SDK cung cấp `SofiPosWebhookVerifier.VerifyAndParse`; ứng dụng đối tác tự triển khai endpoint nhận POST.

1. Đọc nguyên byte body, giữ nguyên trước khi xác minh.
2. Xác minh `X-Webhook-Timestamp` và `X-Webhook-Signature` bằng secret của cửa hàng. Mặc định từ chối timestamp lệch quá 5 phút.
3. Lưu sự kiện có chữ ký hợp lệ và chống trùng theo event ID.
4. Trả 2xx sau khi đã nhận/lưu thành công, xử lý đồng bộ ở worker.
5. Gọi SDK lấy dữ liệu hiện tại theo ID; xử lý `deletedIds` cho bản ghi đã xóa.

Header webhook gồm `X-Webhook-Id`, `X-Webhook-Event`, `X-Webhook-Timestamp`, `X-Webhook-Signature`. Chữ ký dùng HMAC-SHA256 trên `timestamp + "." + rawBody`.

SDK Lab có sẵn endpoint `/webhooks/sofipos`, inbox, mô phỏng chữ ký và worker. Xem [hướng dẫn webhook của demo](../examples/fnb-dotnet-react/README.md#nhận-webhook-thật).

## Phạm vi của SDK Lab

SDK Lab là công cụ thử tích hợp và quan sát dữ liệu. Backend hiện không có đăng nhập hoặc phân quyền người dùng cho các endpoint quản lý; chạy demo trong môi trường phát triển hoặc mạng được kiểm soát. Khi nhận callback từ POS qua Internet, chỉ chuyển tiếp đường dẫn webhook theo hướng dẫn demo.

Thay đổi gửi từ demo qua Partner API không phát webhook từ POS; demo cập nhật bản sao trực tiếp từ response SDK. Để thử callback thật, dùng thao tác trên phần mềm POS hoặc chức năng gửi thử của POS.
