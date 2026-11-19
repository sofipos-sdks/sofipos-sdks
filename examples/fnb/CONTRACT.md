# Hợp đồng HTTP của FnB SDK Lab

Một frontend tại `frontend/` dùng chung cho các backend tại `backends/{language}`. Backend hiện có là .NET; mỗi backend gọi SDK tương ứng, không thay SDK bằng HTTP proxy trực tiếp tới POS.

## Phiên và kết nối

Frontend gọi cùng origin tới `/api/...` và `/webhooks/...`; proxy Vite dùng `BACKEND_URL` (mặc định `http://localhost:5080`). Với frontend tĩnh, reverse proxy thực hiện chuyển tiếp tương tự.

Backend giữ **một phiên trong RAM**, không database, file credential, keyring hoặc snapshot. Khởi động lại mất kết nối và lịch sử. Một backend instance có một kết nối hiện hành.

| Method | Route | Dữ liệu |
|---|---|---|
| GET | `/api/health` | `{ status: "ready", sdk, operations: 92, historyLimit: 200 }` |
| GET | `/api/connection` | Cấu hình công khai, kể cả chưa có phiên |
| POST | `/api/connection/check` | Nhận cấu hình chưa lưu; dùng SDK đọc chi nhánh, trả `{ branches, callId, trace }` |
| PUT | `/api/connection` | Nhận cấu hình, xác minh SDK rồi lưu vào RAM; trả cấu hình công khai |

Cấu hình nhập: `name`, `baseUrl`, `apiKey`, `webhookSecret?`, `clearWebhookSecret?`. Không cần chọn chi nhánh hoặc kênh bán khi kết nối. Key trống giữ key cũ chỉ khi cùng URL; secret trống giữ secret chỉ khi URL/key không đổi. `clearWebhookSecret: true` xóa secret.

Cấu hình công khai: `configured`, `connectionId`, `name`, `baseUrl`, `hasApiKey`, `hasWebhookSecret`, `updatedAt`. Không trả key/secret. Khi chưa có phiên: `configured: false`, hai cờ secret là `false`, ID/thời điểm có thể null.

Đổi URL/key hợp lệ tạo `connectionId` mới và xóa trace/webhook cũ. Xác minh thất bại giữ phiên cũ. Đổi tên/secret giữ ID và lịch sử. Client gửi `X-Workspace-Connection` với ID hiện hành; backend từ chối request từ phiên cũ. Client không tự gửi lại thao tác ghi sau khi lỗi phiên.

Request được điều phối để đổi kết nối không thay credential của lời gọi đang chạy. Thiếu hoặc sai phiên trả `409` với `connectionRequired: true`; frontend đọc lại cấu hình và yêu cầu nhập lại khi backend đã khởi động lại.

## Thao tác SDK và bộ chọn

[COVERAGE.md](COVERAGE.md) liệt kê 92 thao tác. Backend thêm `/api/pos` trước đường dẫn API POS của bảng. [frontend/src/reference.ts](frontend/src/reference.ts) mô tả form path/query/body; mã như `Products.ListAsync` là mã ổn định của giao diện, backend ngôn ngữ khác ánh xạ sang phương thức SDK tương ứng.

Thành công trả `{ data, callId }`. `data` là response gốc SDK: object, array, boolean hoặc null. Danh sách phân trang giữ `data.items`, `data.metaData` với `currentPage`, `totalPages`, `totalCount`, `hasPrevious`, `hasNext`. Không ghi bản sao hoặc gọi API bổ sung để đồng bộ sau thao tác.

Frontend lấy dữ liệu bộ chọn qua chính `/api/pos/...`, với header `X-SDK-Source: lookup`. Các bộ chọn hỗ trợ tìm kiếm/phân trang theo query SDK; danh mục không có tìm kiếm phía server được lọc theo tên sau khi tải. Bản ghi được chọn theo tên nhưng gửi ID thật. Cache bộ chọn chỉ nằm trong RAM FE, xóa khi đổi kết nối hoặc thao tác ghi thành công; không yêu cầu đồng bộ dữ liệu trước.

Các nguồn chính: sản phẩm/khách hàng/combo/kênh bán/danh mục qua API list; chi nhánh/nhóm sản phẩm/hạng thành viên qua lookup; đơn vị qua sản phẩm; nhóm/món combo qua chi tiết combo; bàn/provider/thuộc tính/món kèm qua API FnB; dòng món/combo qua chi tiết đơn. Kênh hệ thống khi tạo kênh bán dùng `SaleChannels.GetOptionsAsync`; kênh dùng trên đơn/thực đơn lấy kênh đang hoạt động.

Mọi FnB request cần `branchId` tường minh theo metadata SDK. `GetMenuAsync` có thể không gửi kênh để dùng mặc định server; tạo đơn gửi `saleChannelId` trong body theo model SDK. Đổi lựa chọn cha không được giữ các ID con thuộc ngữ cảnh cũ. Mã không có API tra cứu và ID do client sinh tiếp tục có control nhập/sinh tương ứng.

## Trace và lỗi

`GET /api/calls?operation=&errorsOnly=false&page=1&callId=` trả `{ items, total, page, pageSize: 20, limit: 200 }`, mới nhất trước. Chỉ giữ 200 HTTP request gần nhất trong RAM; mỗi lần SDK retry có một bản ghi, cùng `callId` của lượt gọi.

API call gồm `id`, `connectionId`, `operation`, `source` (`manual`, `lookup`, `setup`), `callId`, `method`, `url`, `statusCode`, `durationMs`, `traceId`, `error`, `requestJson`, `responseJson`, `headers`, `createdAt`. Các trường JSON body là chuỗi; lỗi mạng có thể không có HTTP status. Key, authorization, secret, token, cookie được che khỏi header; nội dung xác thực và ảnh được che/rút gọn trong body log.

Giữ HTTP status POS, thông báo validation và `traceId`. Lỗi thông thường trả `{ message, traceId, statusCode, callId, connectionRequired }`; validation Problem Details cũng được chấp nhận. Lỗi kiểm tra kết nối trả `{ message, statusCode, callId, trace }`. Không có `syncWarning` hoặc lỗi database trong giao thức này.

JSON dùng UTF-8, thuộc tính camelCase, giữ tiếng Việt và Unicode. GUID là chuỗi, enum giữ giá trị SDK; query mảng dùng khóa lặp. Ngày giờ không có offset được hiểu theo UTC+7 như SDK. Không tự retry POST/PUT.

## Webhook

| Method | Route | Hành vi |
|---|---|---|
| POST | `/webhooks/sofipos` | Xác minh bằng helper SDK, ghi nhận trong RAM, không gọi lại POS |
| GET | `/api/webhooks?state=&page=1` | `{ items, total, page, pageSize: 20, limit: 200 }`, mới nhất trước |
| POST | `/api/webhooks/simulate` | Mô phỏng cục bộ bằng helper ký SDK, dùng cùng luồng nhận |

Callback nhận raw body và các header `X-Webhook-Id`, `X-Webhook-Event`, `X-Webhook-Timestamp`, `X-Webhook-Signature`. Xác minh HMAC và timestamp trong 5 phút, đối chiếu ID/event với body đã ký trước khi ghi inbox. Callback tối đa 1 MiB; payload hiển thị lớn được rút gọn.

- Mới, hợp lệ: `202 { eventId, duplicate: false, message }`, trạng thái `received`.
- Trùng hợp lệ còn trong bộ nhớ: `200 { eventId, duplicate: true, message }`, tăng `receivedCount`.
- Chữ ký sai/quá hạn: `401 { message }`, trạng thái `rejected`, không xử lý nội dung.
- Thiếu secret: `409 { message }`.

Inbox giữ 200 sự kiện gần nhất. Record gồm `id`, `connectionId`, `eventId`, `event`, `payloadJson`, `signatureStatus` (`valid`/`invalid`), `source` (`pos`/`simulated`), `headers`, `state`, `receivedCount`, `error`, `createdAt`. Nhận diện trùng có hiệu lực trong phạm vi các sự kiện còn giữ của phiên.

Mô phỏng nhận `event`, `signatureMode` (`valid`, `invalid`, `expired`), `eventId?`, `orderId?`, `code?`, `branchId?`, `ids: string[]`, `deletedIds: string[]`; trả cùng status/body callback và header `X-Demo-Event-Id`. Chế độ `expired` dùng timestamp cũ 10 phút.

Không có worker, reprocess, lease, snapshot hoặc tự đồng bộ sau webhook. Các API cũ `/api/store/...`, `/api/sync`, `/api/snapshots`, `/api/overview`, `/api/webhooks/{id}/reprocess` đã bỏ.
