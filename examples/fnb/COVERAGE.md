# Độ phủ SDK FnB

**92 / 92** endpoint SDK có lời gọi trực tiếp và form giao diện. Mọi thao tác `Fnb`, `FnbOrders`, `FnbPayments` có trường chi nhánh bắt buộc (query `branchId`).

Frontend dùng chung tại `frontend/`; backend hiện có tại `backends/dotnet/`. Bảng dưới dùng mã thao tác của frontend (giữ tên phương thức SDK .NET hiện tại). Backend ngôn ngữ khác ánh xạ mã này sang SDK tương ứng, giữ nguyên HTTP theo [CONTRACT.md](CONTRACT.md). Đường dẫn backend thêm tiền tố `/api/pos` vào cột API POS.

Menu chính mở các nhóm API và form gửi trực tiếp qua SDK. Thông tin kết nối chỉ giữ trong RAM; không cần database hay đồng bộ danh mục. Các tham chiếu có API danh sách dùng bộ chọn theo tên, tự gửi ID trong request.

| SDK | HTTP | API POS | Vị trí form |
|---|---|---|---|
| `Lookups.GetBranchesAsync` | GET | `/lookups/branches` | Danh mục tra cứu → Chi nhánh |
| `Lookups.GetProductCategoriesAsync` | GET | `/lookups/product-categories` | Danh mục tra cứu → Nhóm sản phẩm |
| `Lookups.GetMembershipRanksAsync` | GET | `/lookups/membership-ranks` | Danh mục tra cứu → Hạng thành viên |
| `Products.ListAsync` | GET | `/products` | Sản phẩm → Danh sách |
| `Products.GetAsync` | GET | `/products/{productId}` | Sản phẩm → Chi tiết |
| `Products.GetUnitsAsync` | GET | `/products/{productId}/units` | Sản phẩm → Đơn vị tính |
| `Products.CreateAsync` | POST | `/products` | Sản phẩm → Tạo mới |
| `Products.UpdateAsync` | PUT | `/products/{productId}` | Sản phẩm → Cập nhật |
| `Products.DeleteAsync` | POST | `/products/{productId}/delete` | Sản phẩm → Xóa |
| `QrOrderCodes.GetBranchesAsync` | GET | `/qr-order-codes/branches` | QR đặt món → Chi nhánh |
| `QrOrderCodes.GetSaleChannelsAsync` | GET | `/qr-order-codes/sale-channels` | QR đặt món → Kênh bán cho QR |
| `QrOrderCodes.GetTablesAsync` | GET | `/qr-order-codes/tables` | QR đặt món → Bàn |
| `QrOrderCodes.GetOrderAttributesAsync` | GET | `/qr-order-codes/order-attributes` | QR đặt món → Thuộc tính đơn |
| `QrOrderCodes.ListAsync` | GET | `/qr-order-codes` | QR đặt món → Danh sách |
| `QrOrderCodes.GetAsync` | GET | `/qr-order-codes/{id}` | QR đặt món → Chi tiết |
| `QrOrderCodes.CreateAsync` | POST | `/qr-order-codes` | QR đặt món → Tạo mới |
| `QrOrderCodes.UpdateAsync` | PUT | `/qr-order-codes/{id}` | QR đặt món → Cập nhật |
| `QrOrderCodes.SetActiveAsync` | PUT | `/qr-order-codes/{id}/active` | QR đặt món → Bật / tắt |
| `QrOrderCodes.GetQrImageAsync` | GET | `/qr-order-codes/{id}/qr` | QR đặt món → Ảnh QR đặt món |
| `QrOrderCodes.DeleteAsync` | POST | `/qr-order-codes/{id}/delete` | QR đặt món → Xóa |
| `Customers.ListAsync` | GET | `/customers` | Khách hàng → Danh sách |
| `Customers.GetAsync` | GET | `/customers/{id}` | Khách hàng → Chi tiết |
| `Customers.CreateAsync` | POST | `/customers` | Khách hàng → Tạo mới |
| `Customers.UpdateAsync` | PUT | `/customers/{id}` | Khách hàng → Cập nhật |
| `Customers.DeleteAsync` | POST | `/customers/{id}/delete` | Khách hàng → Xóa |
| `PricingPolicies.ListAsync` | GET | `/pricing-policies` | Chính sách giá → Danh sách |
| `PricingPolicies.GetAsync` | GET | `/pricing-policies/{id}` | Chính sách giá → Chi tiết |
| `PricingPolicies.GetProductAddOnsAsync` | POST | `/pricing-policies/product-addons` | Chính sách giá → Món kèm |
| `PricingPolicies.CreateAsync` | POST | `/pricing-policies` | Chính sách giá → Tạo mới |
| `PricingPolicies.UpdateAsync` | PUT | `/pricing-policies/{id}` | Chính sách giá → Cập nhật |
| `PricingPolicies.DeleteAsync` | POST | `/pricing-policies/{id}/delete` | Chính sách giá → Xóa |
| `SaleChannels.GetOptionsAsync` | GET | `/sale-channels/options` | Kênh bán → Kênh bán có thể thêm |
| `SaleChannels.ListAsync` | GET | `/sale-channels` | Kênh bán → Danh sách |
| `SaleChannels.GetAsync` | GET | `/sale-channels/{saleChannelId}` | Kênh bán → Chi tiết |
| `SaleChannels.CreateAsync` | POST | `/sale-channels` | Kênh bán → Tạo mới |
| `SaleChannels.DeleteAsync` | POST | `/sale-channels/{saleChannelId}/delete` | Kênh bán → Xóa |
| `Combos.ListAsync` | GET | `/combos` | Combo → Danh sách |
| `Combos.GetAsync` | GET | `/combos/{id}` | Combo → Chi tiết |
| `Combos.CreateAsync` | POST | `/combos` | Combo → Tạo mới |
| `Combos.UpdateAsync` | PUT | `/combos/{id}` | Combo → Cập nhật |
| `Combos.DeleteAsync` | POST | `/combos/{id}/delete` | Combo → Xóa |
| `FlashSales.ListAsync` | GET | `/flashsales` | Flash sale → Danh sách |
| `FlashSales.GetAsync` | GET | `/flashsales/{id}` | Flash sale → Chi tiết |
| `FlashSales.CreateAsync` | POST | `/flashsales` | Flash sale → Tạo mới |
| `FlashSales.UpdateAsync` | PUT | `/flashsales/{id}` | Flash sale → Cập nhật |
| `FlashSales.DeleteAsync` | POST | `/flashsales/{id}/delete` | Flash sale → Xóa |
| `Vouchers.ListAsync` | GET | `/vouchers` | Voucher → Danh sách |
| `Vouchers.GetAsync` | GET | `/vouchers/{id}` | Voucher → Chi tiết |
| `Vouchers.GetCodesAsync` | GET | `/vouchers/{id}/codes` | Voucher → Danh sách mã voucher |
| `Vouchers.CreateAsync` | POST | `/vouchers` | Voucher → Tạo mới |
| `Vouchers.UpdateAsync` | PUT | `/vouchers/{id}` | Voucher → Cập nhật |
| `Vouchers.SetActiveAsync` | PUT | `/vouchers/{id}/active` | Voucher → Bật / tắt |
| `Vouchers.DeleteAsync` | POST | `/vouchers/{id}/delete` | Voucher → Xóa |
| `OrderAdjustmentPrograms.ListAsync` | GET | `/order-adjustment-programs` | Chiết khấu / phụ thu → Danh sách |
| `OrderAdjustmentPrograms.GetAsync` | GET | `/order-adjustment-programs/{id}` | Chiết khấu / phụ thu → Chi tiết |
| `OrderAdjustmentPrograms.CreateAsync` | POST | `/order-adjustment-programs` | Chiết khấu / phụ thu → Tạo mới |
| `OrderAdjustmentPrograms.UpdateAsync` | PUT | `/order-adjustment-programs/{id}` | Chiết khấu / phụ thu → Cập nhật |
| `OrderAdjustmentPrograms.DeleteAsync` | POST | `/order-adjustment-programs/{id}/delete` | Chiết khấu / phụ thu → Xóa |
| `Fnb.GetAreasAsync` | GET | `/fnb/areas` | Dữ liệu bán FnB → Khu vực & bàn |
| `Fnb.GetMenuAsync` | GET | `/fnb/menu` | Dữ liệu bán FnB → Menu bán hàng |
| `Fnb.GetProductAddOnsAsync` | GET | `/fnb/products/{productId}/addons` | Dữ liệu bán FnB → Món kèm |
| `Fnb.GetProductAttributesAsync` | GET | `/fnb/products/{productId}/attributes` | Dữ liệu bán FnB → Thuộc tính món |
| `Fnb.GetPaymentOptionsAsync` | GET | `/fnb/payment-options` | Dữ liệu bán FnB → Phương thức thanh toán |
| `FnbOrders.CreateAsync` | POST | `/fnb/orders` | Đơn hàng FnB → Tạo mới |
| `FnbOrders.ListAsync` | GET | `/fnb/orders` | Đơn hàng FnB → Danh sách |
| `FnbOrders.GetAsync` | GET | `/fnb/orders/{orderId}` | Đơn hàng FnB → Chi tiết |
| `FnbOrders.UpdateNoteAsync` | PUT | `/fnb/orders/{orderId}/note` | Đơn hàng FnB → Ghi chú đơn |
| `FnbOrders.SetCustomerAsync` | PUT | `/fnb/orders/{orderId}/customer` | Đơn hàng FnB → Khách của đơn |
| `FnbOrders.CancelAsync` | POST | `/fnb/orders/{orderId}/cancel` | Đơn hàng FnB → Hủy đơn |
| `FnbOrders.AddProductAsync` | POST | `/fnb/orders/{orderId}/products` | Đơn hàng FnB → Thêm một món |
| `FnbOrders.AddProductsAsync` | POST | `/fnb/orders/{orderId}/products/bulk` | Đơn hàng FnB → Thêm nhiều món |
| `FnbOrders.UpdateProductQuantityAsync` | PUT | `/fnb/orders/{orderId}/products/{lineId}/quantity` | Đơn hàng FnB → Sửa số lượng món |
| `FnbOrders.UpdateProductUnitAsync` | PUT | `/fnb/orders/{orderId}/products/{lineId}/unit` | Đơn hàng FnB → Đổi đơn vị tính |
| `FnbOrders.UpdateProductPriceAsync` | PUT | `/fnb/orders/{orderId}/products/{lineId}/price` | Đơn hàng FnB → Đổi giá món |
| `FnbOrders.UpdateProductNoteAsync` | PUT | `/fnb/orders/{orderId}/products/{lineId}/note` | Đơn hàng FnB → Ghi chú món |
| `FnbOrders.UpdateProductAttributesAsync` | PUT | `/fnb/orders/{orderId}/products/{lineId}/attributes` | Đơn hàng FnB → Thuộc tính món |
| `FnbOrders.UpdateProductAddOnsAsync` | PUT | `/fnb/orders/{orderId}/products/{lineId}/addons` | Đơn hàng FnB → Món kèm của dòng |
| `FnbOrders.DeleteProductAsync` | POST | `/fnb/orders/{orderId}/products/{lineId}/delete` | Đơn hàng FnB → Xóa dòng món |
| `FnbOrders.CancelProductAsync` | POST | `/fnb/orders/{orderId}/products/{lineId}/cancel` | Đơn hàng FnB → Hủy món đã chế biến |
| `FnbOrders.AddComboAsync` | POST | `/fnb/orders/{orderId}/combos` | Đơn hàng FnB → Thêm một combo |
| `FnbOrders.AddCombosAsync` | POST | `/fnb/orders/{orderId}/combos/bulk` | Đơn hàng FnB → Thêm nhiều combo |
| `FnbOrders.UpdateComboAsync` | PUT | `/fnb/orders/{orderId}/combos/{lineId}` | Đơn hàng FnB → Sửa dòng combo |
| `FnbOrders.DeleteComboAsync` | POST | `/fnb/orders/{orderId}/combos/{lineId}/delete` | Đơn hàng FnB → Xóa dòng combo |
| `FnbOrders.CancelComboAsync` | POST | `/fnb/orders/{orderId}/combos/{lineId}/cancel` | Đơn hàng FnB → Hủy combo |
| `FnbOrders.ChangeTableAsync` | PUT | `/fnb/orders/{orderId}/table-sale-channel` | Đơn hàng FnB → Đổi bàn / kênh bán |
| `FnbOrders.MergeAsync` | POST | `/fnb/orders/merge` | Đơn hàng FnB → Gộp đơn |
| `FnbOrders.MoveItemsAsync` | POST | `/fnb/orders/{orderId}/move-items` | Đơn hàng FnB → Chuyển / tách món |
| `FnbPayments.PreviewAsync` | POST | `/fnb/orders/{orderId}/payment/preview` | Thanh toán & QR → Tạm tính |
| `FnbPayments.CreateQrAsync` | POST | `/fnb/orders/{orderId}/qr` | Thanh toán & QR → Tạo QR thanh toán |
| `FnbPayments.CheckQrTransactionAsync` | GET | `/fnb/orders/{orderId}/qr/check/{provider}` | Thanh toán & QR → Kiểm tra chuyển khoản |
| `FnbPayments.PayAsync` | PUT | `/fnb/orders/{orderId}/payment` | Thanh toán & QR → Ghi nhận thanh toán |
| `SaleChannels.UpdateAsync` | PUT | `/sale-channels/{saleChannelId}` | Kênh bán → Cập nhật |

Bộ chọn dùng API list/lookup và chi tiết qua SDK, hỗ trợ tìm kiếm và phân trang; không tạo endpoint ghi mới.
SDK verifier, chữ ký, ảnh QR và xử lý lỗi được dùng trực tiếp.
