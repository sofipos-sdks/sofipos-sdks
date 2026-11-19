# SofiPOS FnB SDK Lab

Ứng dụng thử **92 thao tác / 13 nhóm SDK FnB** bằng form. Nhập thông tin kết nối, chọn thao tác, điền dữ liệu rồi gửi qua SDK thật. Những trường như sản phẩm, khách hàng, combo, chi nhánh, kênh bán, bàn và đơn hàng chọn theo tên từ API; không phải nhập ID tham chiếu.

## Cấu trúc

```text
examples/fnb/
├── frontend/          # Một frontend React dùng chung
├── backends/
│   └── dotnet/        # ASP.NET Core gọi SDK .NET
├── CONTRACT.md        # Hợp đồng HTTP cho các backend
├── COVERAGE.md        # Đối chiếu từng API và form
└── README.md
```

Backend hiện có là .NET; chưa có backend Node.js. Khi triển khai SDK khác, thêm `backends/{language}` và giữ cùng [hợp đồng HTTP](CONTRACT.md); không sao chép frontend.

## Chạy trên Windows

Cần .NET SDK 10 và Node.js 22.12+. Không cần MongoDB, database, Docker hoặc keyring.

Từ thư mục gốc repository, chạy backend:

```powershell
Push-Location examples/fnb/backends/dotnet
dotnet restore
dotnet run
Pop-Location
```

Backend mặc định tại `http://localhost:5080`. Chạy frontend ở terminal khác:

```powershell
Push-Location examples/fnb/frontend
npm ci
npm run dev
Pop-Location
```

Mở URL mà Vite in trong terminal (mặc định `http://127.0.0.1:5173`; nếu cổng bận, Vite dùng cổng tiếp theo). Lệnh dev đã tắt mã màu ANSI và thao tác xóa màn hình để log hiển thị được trong cửa sổ không hỗ trợ terminal màu.

Frontend chuyển tiếp `/api` và `/webhooks` tới backend. Để đổi backend, sao chép `frontend/.env.example` thành `.env.local`, đặt `BACKEND_URL` rồi khởi động lại Vite. URL này là backend SDK Lab, khác với API Base URL của cửa hàng.

```powershell
Push-Location examples/fnb/frontend
$env:BACKEND_URL = 'http://localhost:5081'
npm run dev
Pop-Location
```

Với frontend tĩnh, cấu hình reverse proxy cho `/api` và `/webhooks`; biến `BACKEND_URL` chỉ dùng cho proxy Vite khi phát triển.

## Kết nối và phiên

1. Nhập tên kết nối, API Base URL và Partner Key; thêm secret webhook nếu cần.
2. Bấm **Kiểm tra qua SDK** để xem kết quả và trace của `Lookups.GetBranchesAsync`.
3. Bấm **Lưu kết nối**. Backend xác minh qua SDK trước khi thay phiên; không cần chọn chi nhánh hoặc kênh bán trong setup.
4. Chọn nhóm API và thao tác cần thử.

Base URL có thể kèm `/api/partner/v1`; SDK không nối lặp tiền tố. API HTTPS dùng xác minh TLS bình thường.

Thông tin xác thực chỉ nằm trong RAM backend, không trả lại cho frontend sau khi lưu. Tải lại trang vẫn dùng được phiên hiện hành; khởi động lại backend phải nhập lại kết nối. Một backend có một kết nối hiện hành dùng chung cho các cửa sổ FE nối tới nó.

Đổi URL hoặc Partner Key hợp lệ sẽ tạo phiên mới, xóa trace/webhook cũ và cache bộ chọn. Đổi thông tin sai giữ nguyên phiên cũ. Để trống key chỉ giữ được key cùng URL; secret trống chỉ giữ khi danh tính kết nối không đổi. Đổi tên hoặc secret không xóa lịch sử. Dữ liệu POS không bị xóa khi đổi kết nối.

## Gọi API và chọn dữ liệu

- Mỗi nhóm có danh sách thao tác, form path/query/body, request xem trước, kết quả và JSON response đầy đủ.
- Bộ chọn gọi SDK khi mở hoặc cần tải tên bản ghi đã chọn. Có tìm kiếm, phân trang và tải lại; dữ liệu chỉ cache trong RAM FE, không tự polling danh mục.
- Chọn sản phẩm để tải đơn vị tính, thuộc tính và món kèm; chọn combo để tải nhóm và món trong nhóm; chọn đơn để tải dòng món/combo.
- Chọn chi nhánh khi thao tác cần `branchId`. Đổi chi nhánh hoặc lựa chọn cha sẽ bỏ các lựa chọn con cũ. Kênh bán của thực đơn có thể bỏ trống để dùng mặc định của POS.
- Các ID tham chiếu được gửi tự động sau khi chọn tên. `requestId` và ID do client sinh có nút tạo GUID; mã không có API tra cứu vẫn có trường nhập tương ứng. ID thực có thể xem trong request/trace.
- Với dữ liệu lồng nhau, dùng **Thêm dòng** và nút xóa dòng. API nhận ảnh dùng bộ chọn file.
- Lỗi lấy danh sách hiển thị ngay trong bộ chọn, có nút thử lại; không thay bằng lựa chọn giả hoặc buộc nhập ID.

Các thao tác ghi tác động trực tiếp POS. SDK giữ cơ chế retry GET; ứng dụng không tự gửi lại POST/PUT. Tạo đơn giữ `requestId` khi gửi lại cùng yêu cầu; **Đặt lại form** sinh request mới. Tạo QR hoặc kiểm tra nhận tiền không tự ghi nhận thanh toán; thao tác thanh toán có form riêng. Kiểm tra giao dịch QR có thể bật polling mỗi 4 giây và dừng khi nhận tiền hoặc có lỗi.

## Trace và webhook

**Lịch sử gọi SDK** giữ tối đa **200 HTTP request** trong RAM, gồm cả các lần SDK retry. Xem method, URL/query, header đã che key/secret, body, response, thời gian và `traceId`. Nút **Xem trace gửi POS** / **Xem trace lỗi** lọc lịch sử theo lượt gọi. Các request của bộ chọn có nguồn `lookup`.

Callback POS: `POST /webhooks/sofipos`. Khi POS ở máy khác, dùng URL HTTPS công khai chuyển tiếp đường dẫn này và cấu hình `AllowedHosts` phù hợp nếu reverse proxy giữ Host gốc.

Backend dùng `SofiPosWebhookVerifier.VerifyAndParse` xác minh raw body, chữ ký và timestamp (lệch tối đa 5 phút), đối chiếu ID/event giữa header với body rồi ghi inbox trong RAM. Sự kiện mới trả `202`; sự kiện trùng hợp lệ trả `200` và tăng số lần nhận; chữ ký sai/quá hạn trả `401` và được ghi là từ chối.

Inbox giữ tối đa **200 sự kiện**. Nhận diện trùng chỉ áp dụng các sự kiện hợp lệ còn được giữ trong phiên. Nhận webhook không gọi lại POS, không đồng bộ và không có worker xử lý lại. Payload lớn và HTTP log được rút gọn theo giới hạn log; callback giới hạn 1 MiB.

Form mô phỏng dùng helper SDK để ký và đi qua cùng luồng xác minh: hợp lệ, sai chữ ký, timestamp cũ 10 phút và trùng event ID. Việc mô phỏng không gửi HTTP ra ngoài; các bộ chọn dữ liệu trong form vẫn gọi SDK khi người dùng mở chúng.

## Kiểm tra source

```powershell
Push-Location examples/fnb/frontend
npm run check
Pop-Location
```

Lệnh chỉ kiểm tra TypeScript (`tsc --noEmit`). Không tự chạy test, full build hoặc ghi dữ liệu thử lên POS. Không có database migration; database, file cấu hình local và keyring của demo cũ không bị xóa nhưng ứng dụng không dùng chúng để lưu phiên nữa.
