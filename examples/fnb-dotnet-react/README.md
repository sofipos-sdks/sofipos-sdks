# SofiPOS FnB SDK Lab

Ứng dụng demo dùng **SDK .NET thật** để thao tác trên cửa hàng SofiPOS. Frontend React có form cho đủ **91 endpoint / 13 nhóm SDK**. Không nhập JSON để thực hiện thao tác; JSON chỉ dùng để xem request, response và dữ liệu đã lưu.

## Thành phần

| Thành phần | Công nghệ | Địa chỉ mặc định |
|---|---|---|
| Backend | ASP.NET Core .NET 10, MongoDB Driver | `http://localhost:5080` |
| Frontend | React, TypeScript, Vite | `http://127.0.0.1:5173` |
| SDK | `SofiPOS.FnB.Sdk`, target .NET 8 | `../../sdks/fnb/dotnet` |
| MongoDB | Database riêng của demo | `sofipos_sdk_demo` |

Backend tham chiếu trực tiếp SDK qua `ProjectReference`. Các controller gọi phương thức SDK tường minh, không chuyển tiếp HTTP bằng một proxy thay thế SDK. Xem [COVERAGE.md](COVERAGE.md) để đối chiếu từng hàm, endpoint và form.

## Chạy trên Windows

Cần .NET SDK 10, Node.js 22.12+ và một MongoDB đang chạy. MongoDB có thể đặt trên máy local hoặc ở một máy chủ khác qua connection string; demo không tự cài MongoDB hoặc chạy Docker.

Các khối lệnh bên dưới bắt đầu từ thư mục gốc repository đã clone. `Push-Location` và `Pop-Location` giúp quay về thư mục gốc sau mỗi bước.

### 1. Cấu hình MongoDB

Trong PowerShell:

```powershell
Push-Location examples/fnb-dotnet-react/backend
Copy-Item appsettings.Development.example.json appsettings.Development.json
Pop-Location
```

Sửa `Mongo:ConnectionString` và `Mongo:Database` trong `appsettings.Development.json`. Nếu MongoDB local ở cổng 27017 thì giữ mặc định.

Cũng có thể đặt cấu hình bằng biến môi trường:

```powershell
$env:Mongo__ConnectionString = 'mongodb://127.0.0.1:27017'
$env:Mongo__Database = 'sofipos_sdk_demo'
```

Không đưa connection string có thông tin đăng nhập vào Git. `appsettings.Development.json` đã nằm trong `.gitignore`.

### 2. Chạy backend

```powershell
Push-Location examples/fnb-dotnet-react/backend
dotnet restore
dotnet run
Pop-Location
```

Launch profile đặt môi trường `Development`, đọc `appsettings.Development.json` và lắng nghe ở `http://localhost:5080`.

`GET http://localhost:5080/api/health` trả `ready` khi MongoDB khả dụng. Nếu MongoDB chưa chạy, backend vẫn mở nhưng trả `503` cho chức năng cần lưu dữ liệu; worker tự kết nối lại.

### 3. Chạy frontend ở terminal khác

```powershell
Push-Location examples/fnb-dotnet-react/frontend
npm ci
npm run dev
Pop-Location
```

Mở **http://127.0.0.1:5173**. Vite chuyển tiếp `/api` và `/webhooks` tới backend, không cần cấu hình CORS thủ công.

Nếu đổi cổng backend, cập nhật cả `backend/appsettings.json` hoặc launch profile và `frontend/vite.config.ts`.

### 4. Kết nối POS

Mở **Kết nối cửa hàng**:

1. Điền tên, API Base URL và Partner Key của cửa hàng, rồi lưu.
2. Bấm **Kiểm tra kết nối đã lưu**. Demo gọi `Lookups.GetBranchesAsync` qua SDK và tải danh mục cho các bộ chọn.
3. Chọn một chi nhánh FnB, kênh bán mặc định và lưu lại.
4. Điền secret webhook `whsec_...` do cửa hàng cấp nếu muốn nhận webhook.

Base URL có thể là `https://api.example.com` hoặc đã kèm `/api/partner/v1`; SDK không nối lặp tiền tố. API local HTTPS cần chứng chỉ được hệ điều hành tin cậy; demo giữ cơ chế xác minh TLS bình thường.

Partner Key và secret không được trả lại cho frontend sau khi lưu. Cấu hình MongoDB giữ dữ liệu đã mã hóa bằng ASP.NET Core Data Protection; keyring nằm ở `backend/.local-data/keys` và được loại khỏi Git. Giữ keyring khi chạy lại hoặc chuyển máy; nếu mất, nhập lại Partner Key và secret tại màn hình kết nối.

Một kết nối được chọn làm hiện hành. Đổi URL hoặc Partner Key tạo `connectionId` mới để dữ liệu của hai cửa hàng không trộn lẫn; dữ liệu/lịch sử cũ không bị xóa. Đổi URL phải nhập lại Partner Key. Khi thay secret webhook có thể giữ nguyên Partner Key.

## Các màn hình

- **Tổng quan:** trạng thái kết nối, số bản sao, số lượt gọi SDK, webhook và hoạt động gần đây.
- **Danh mục tra cứu:** chi nhánh và hạng thành viên.
- **Sản phẩm:** danh sách, chi tiết, đơn vị tính, tạo/sửa/xóa.
- **QR đặt món:** chi nhánh/kênh/bàn/thuộc tính; CRUD, bật/tắt và ảnh QR.
- **Khách hàng:** lọc, phân trang, chi tiết và CRUD.
- **Chính sách giá:** phạm vi, sản phẩm, đơn vị, món kèm, ngày trong tuần và khung giờ.
- **Combo:** nhóm lựa chọn, sản phẩm, số lượng, đơn vị và ảnh.
- **Flash sale:** thời gian, phạm vi và các dòng sản phẩm/đơn vị/giá.
- **Kênh bán:** danh sách hệ thống có thể thêm, chi tiết và CRUD.
- **Voucher:** cấu hình giảm giá, mã nhập tay hoặc sinh mã, danh sách mã, bật/tắt và CRUD.
- **Chiết khấu / phụ thu:** loại, mức áp dụng, phạm vi, lịch, ưu tiên và hạng thành viên.
- **Dữ liệu bán FnB:** khu vực/bàn, menu, thuộc tính, món kèm và phương thức thanh toán.
- **Đơn FnB:** tạo đơn, thêm một/nhiều món hoặc combo, sửa các thuộc tính dòng, xóa/hủy, đổi khách/ghi chú/bàn/kênh, gộp hoặc tách đơn.
- **Thanh toán:** tạm tính, ưu đãi, tạo/hiển thị QR, kiểm tra chuyển khoản và chốt đơn.
- **Đồng bộ, MongoDB, webhook và lịch sử:** xem tiến trình, bản sao, sự kiện, payload và từng HTTP request SDK.

Trong mỗi nhóm, chọn thao tác ở cột trái rồi điền form. ID có thể chọn theo tên từ bộ chọn hoặc nhập trực tiếp. Dùng **Tải danh mục chọn** sau khi tạo dữ liệu mới để cập nhật dropdown. Khi chọn sản phẩm, demo đọc đơn vị tính, thuộc tính và món kèm; khi chọn combo, demo đọc nhóm lựa chọn.

Với các dòng lồng nhau, dùng **Thêm dòng** và biểu tượng xóa dòng. Các GUID mới của `id`, `requestId`, `targetId` có nút sinh tự động. Mọi trường của model request SDK đều có control trên form; dữ liệu được chuyển qua đúng model .NET trước khi gọi SDK.

Request/response có thể xem ở panel JSON chỉ đọc. Lỗi `400/401/404/422`, nội dung validation và `traceId` được giữ để kiểm tra. Dữ liệu response gốc từ SDK nằm trong trường `data` của phản hồi demo, kèm `callId` và `syncWarning` khi việc lưu MongoDB không thành công.

## Luồng FnB gợi ý

1. Đọc menu, bàn và phương thức thanh toán của chi nhánh.
2. Tạo đơn; chọn sản phẩm, đơn vị tính, số lượng, món kèm/thuộc tính và combo.
3. Giữ `requestId` khi gửi lại cùng một yêu cầu tạo đơn. **Đặt lại form** sinh request ID cho một đơn mới.
4. Mở chi tiết đơn. Mỗi dòng món/combo có bộ chọn thao tác để điền sẵn `orderId`, `lineId` và số lượng.
5. Dùng form đổi bàn, gộp hoặc tách đơn nếu cần. Demo đọc lại đơn nguồn/đích sau khi chuyển món để cập nhật bản sao.
6. Bấm **Thanh toán** để mở tạm tính. Chuyển số phải thu sang form QR hoặc ghi nhận thanh toán.
7. Tạo QR và kiểm tra giao dịch bằng provider lấy từ cấu hình chi nhánh; có thể bật kiểm tra lại mỗi 4 giây. Khi đã nhận tiền hoặc có lỗi, việc kiểm tra tự dừng.
8. Ghi nhận thanh toán bằng nút gửi trên form. Việc tạo QR hoặc nhận kết quả kiểm tra không tự chốt đơn.

Các thao tác gửi SDK sẽ tác động đến cửa hàng đã kết nối. SDK tự retry các GET tạm lỗi theo cấu hình sẵn có; demo không tự gửi lại POST/PUT. Nếu POS đã xử lý thành công nhưng MongoDB lỗi, giao diện báo cảnh báo đồng bộ; dùng GET hoặc **Đồng bộ dữ liệu** để đọc lại, không cần gửi lại thao tác ghi.

## Đồng bộ MongoDB

Các collection được tạo trong database của demo:

| Collection | Nội dung |
|---|---|
| `connections` | Kết nối hiện hành, key và secret đã mã hóa |
| `snapshots` | Bản sao theo `connectionId`, nhóm, ID và ngữ cảnh |
| `api_calls` | HTTP của SDK, trạng thái, thời gian, `traceId`, request/response đã che key |
| `webhook_events` | Payload, header, chữ ký, số lần nhận và trạng thái xử lý |
| `sync_runs` | Nhóm đã chọn, tiến trình, số bản ghi và lỗi |

Chọn các nhóm ở màn hình **Đồng bộ dữ liệu**. Các danh mục được duyệt hết trang bằng helper `ListAllAsync`; mỗi bản ghi được gọi `GetAsync` để lưu chi tiết. Mã voucher được đọc đầy đủ các trang. Đồng bộ đơn yêu cầu chi nhánh FnB và khoảng thời gian, mặc định hôm nay theo UTC+7.

Danh sách có thể lưu bản tóm tắt cho bản ghi mới; bản tóm tắt không ghi đè bản chi tiết đã lưu. Không xóa dữ liệu chỉ vì một trang hoặc một khoảng lọc không trả về bản ghi đó. Thao tác xóa thành công và webhook `deletedIds` sẽ xóa bản sao tương ứng.

Worker đồng bộ và worker webhook chạy độc lập bằng `BackgroundService`; không cần thêm Hangfire hoặc SQL cho ứng dụng demo. Trạng thái đang chạy có lease lưu trong MongoDB, có thể lấy lại sau khi tiến trình dừng. Phiên gắn với kết nối đã thay đổi sẽ báo lỗi thay vì tiếp tục dùng phiên cũ.

## Nhận webhook thật

Endpoint backend:

```http
POST http://localhost:5080/webhooks/sofipos
```

Khi POS ở máy chủ khác, cấu hình một URL HTTPS công khai chuyển tiếp **đường dẫn callback** về endpoint này. Thêm tên miền đó vào `AllowedHosts` trong cấu hình backend nếu reverse proxy giữ Host gốc. Không cần công khai các endpoint quản lý demo.

Nhập URL vào cấu hình webhook trên phần mềm POS, bật gửi webhook và bấm **Gửi thử**. Với việc thử hoàn toàn trên máy local, dùng chế độ validation dành cho phát triển ở POS nếu cần URL HTTP/local; demo không tự thay đổi cấu hình POS.

Backend xử lý:

1. Đọc raw body, xác minh bằng `SofiPosWebhookVerifier.VerifyAndParse` và secret của kết nối hiện hành. Timestamp cho phép lệch tối đa 5 phút.
2. Đối chiếu event ID/loại sự kiện trong header với body đã ký.
3. Lưu vào MongoDB rồi trả `202`, không chờ gọi POS để đồng bộ trong request callback.
4. ID sự kiện trùng trả `200`, tăng số lần nhận và không xử lý lại.
5. Worker gọi SDK lấy dữ liệu mới nhất, xử lý bản ghi xóa và làm mới các snapshot liên quan. Lỗi được hiển thị trong inbox; chọn **Xử lý lại** để chạy lại thao tác đọc.

`ping` không gọi API POS. Sự kiện chưa biết được lưu và đánh dấu bỏ qua. Chữ ký sai/quá hạn được ghi nhận là từ chối và trả `401`; không đồng bộ dữ liệu của payload đó. Nếu MongoDB chưa lưu được sự kiện, endpoint không trả xác nhận thành công.

Thay đổi được gửi từ ứng dụng demo qua Partner API không phát webhook từ POS; demo cập nhật bản sao trực tiếp từ response SDK. Muốn thử callback thật, thay đổi danh mục/đơn trên phần mềm POS hoặc dùng nút **Gửi thử** của POS.

## Mô phỏng webhook cục bộ

Màn hình **Webhook inbox** có form mô phỏng mọi event SDK khai báo:

- Hợp lệ: ký bằng `ComputeSignature` của SDK rồi đi qua cùng hàm xác minh/lưu sự kiện của endpoint callback.
- Sai chữ ký: trả `401` và xuất hiện trong inbox ở trạng thái từ chối.
- Timestamp quá hạn: ký bằng timestamp cũ 10 phút, verifier từ chối.
- Gửi trùng: giữ event ID, gửi lại; số lần nhận tăng nhưng chỉ xử lý một sự kiện.

Mô phỏng là xử lý cục bộ trong backend, không gửi HTTP tới một dịch vụ bên ngoài. Event thay đổi hợp lệ gọi GET thật để đọc bản ghi theo ID; có thể dùng `ping` để thử chữ ký mà không cần gọi API POS. Callback HTTP thật được kiểm tra bằng nút Gửi thử trên POS.

## API nội bộ của demo

| Endpoint | Chức năng |
|---|---|
| `GET /api/health` | Trạng thái MongoDB/backend |
| `GET/PUT /api/connection` | Xem cấu hình đã che key / lưu cấu hình |
| `POST /api/connection/check` | Gọi SDK kiểm tra kết nối |
| `/api/pos/...` | 91 thao tác SDK, đường dẫn tương ứng Partner API |
| `POST /api/sync` | Tạo phiên đồng bộ |
| `GET /api/sync`, `GET /api/sync/{id}` | Tiến trình đồng bộ |
| `GET /api/snapshots` | Xem bản sao, lọc nhóm/ID/phân trang |
| `GET /api/calls` | Lịch sử SDK, lọc hàm/lỗi/phân trang |
| `GET /api/overview` | Thống kê workspace |
| `POST /webhooks/sofipos` | Nhận webhook đã ký |
| `GET /api/webhooks` | Inbox có lọc trạng thái và phân trang |
| `POST /api/webhooks/simulate` | Mô phỏng sự kiện bằng helper SDK |
| `POST /api/webhooks/{id}/reprocess` | Xử lý lại sự kiện hợp lệ |

## Kịch bản nghiệm thu thủ công

1. Cấu hình MongoDB; kiểm tra trang Tổng quan và trạng thái kết nối.
2. Lưu Partner Key, kiểm tra API và chọn chi nhánh FnB.
3. Qua từng nhóm danh mục: danh sách/phân trang/bộ lọc, tạo, mở chi tiết, sửa và xóa. Kiểm tra cả form nhóm lựa chọn, lịch, phạm vi và ảnh.
4. Kiểm tra QR đặt món, bật/tắt QR, lấy ảnh; voucher bật/tắt và duyệt danh sách mã; các danh mục tra cứu chỉ đọc.
5. Tạo đơn có món, món kèm, thuộc tính và combo; thao tác mọi dòng; chuyển/gộp/tách; tạm tính, QR và thanh toán.
6. Kiểm tra lỗi nghiệp vụ như ID không tồn tại, dữ liệu chưa đủ hoặc sửa đơn đã đóng: UI giữ form và hiển thị lỗi từ API.
7. Đồng bộ qua nhiều trang; xem bản chi tiết và ngữ cảnh trong MongoDB.
8. Thử webhook hợp lệ, chữ ký sai, timestamp cũ, trùng event ID và lỗi lấy dữ liệu. Xử lý lại một sự kiện lỗi.
9. Dừng backend lúc còn công việc chờ, chạy lại và quan sát worker lấy lại công việc sau khi lease hết hạn.
10. Đối chiếu từng thao tác ở màn hình **Độ phủ chức năng** với [COVERAGE.md](COVERAGE.md).

Không có thao tác tự động ghi dữ liệu thử vào POS khi cài đặt hoặc mở demo. Hướng dẫn trên là các bước chạy thủ công, không phải test tự động đã được chạy.

## Kiểm tra source

```powershell
Push-Location examples/fnb-dotnet-react/frontend
npm run check
Pop-Location
```

Lệnh này chỉ kiểm tra TypeScript (`tsc --noEmit`). Không cần build frontend để chạy Vite development server. Dự án không thêm database migration hoặc bộ test tự động mới; giữ nguyên bộ test sẵn có của SDK ở vị trí đã chuyển.
