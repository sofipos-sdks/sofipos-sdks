namespace SofiPOS.FnB.Models;

/// <summary>Lọc danh sách sản phẩm.</summary>
public sealed class ProductListQuery : PageQuery
{
    /// <summary>Tìm theo mã hoặc tên sản phẩm (không phân biệt hoa thường).</summary>
    public string? Keyword { get; set; }
}

/// <summary>Lọc danh sách mã QR đặt món.</summary>
public sealed class QrOrderCodeListQuery : PageQuery
{
    /// <summary>Chi nhánh.</summary>
    public Guid? BranchId { get; set; }

    /// <summary>Kênh bán.</summary>
    public string? SaleChannelId { get; set; }

    /// <summary>Trạng thái: <c>null</c> tất cả, 1 đang hiệu lực, 2 đã tắt, 3 hết hạn.</summary>
    public int? Status { get; set; }

    /// <summary>Tìm theo tên QR hoặc tên bàn.</summary>
    public string? Keyword { get; set; }
}

/// <summary>Lọc danh sách khách hàng.</summary>
public sealed class CustomerListQuery : PageQuery
{
    /// <summary>Từ khóa (tên, số điện thoại).</summary>
    public string? Key { get; set; }

    /// <summary>Từ ngày.</summary>
    public DateTime? FromDate { get; set; }

    /// <summary>Đến ngày.</summary>
    public DateTime? ToDate { get; set; }

    /// <summary>Hạng thành viên.</summary>
    public Guid? RankId { get; set; }
}

/// <summary>Lọc danh sách chính sách giá.</summary>
public sealed class PricingPolicyListQuery : PageQuery
{
    /// <summary>Từ khóa theo tên.</summary>
    public string? Key { get; set; }

    /// <summary>Trạng thái hoạt động.</summary>
    public bool? IsActive { get; set; }

    /// <summary>Chi nhánh áp dụng.</summary>
    public Guid? BranchId { get; set; }
}

/// <summary>Lọc danh sách kênh bán của cửa hàng.</summary>
public sealed class SaleChannelListQuery : PageQuery
{
    /// <summary>Tên kênh bán.</summary>
    public string? Name { get; set; }

    /// <summary>Trạng thái hoạt động.</summary>
    public bool? IsActive { get; set; }
}

/// <summary>Lọc danh sách combo.</summary>
public sealed class ComboListQuery : PageQuery
{
    /// <summary>Từ khóa theo tên.</summary>
    public string? Key { get; set; }

    /// <summary>Trạng thái hoạt động.</summary>
    public bool? IsActive { get; set; }
}

/// <summary>Trạng thái lọc flash sale.</summary>
public enum FlashSaleStatusFilter
{
    /// <summary>Tất cả.</summary>
    All = 0,

    /// <summary>Đang chạy.</summary>
    Running = 1,

    /// <summary>Tạm dừng.</summary>
    Paused = 2,

    /// <summary>Sắp diễn ra.</summary>
    Upcoming = 3,

    /// <summary>Đã kết thúc.</summary>
    Ended = 4
}

/// <summary>Lọc danh sách flash sale.</summary>
public sealed class FlashSaleListQuery : PageQuery
{
    /// <summary>Từ khóa theo tên.</summary>
    public string? Key { get; set; }

    /// <summary>Trạng thái hoạt động.</summary>
    public bool? IsActive { get; set; }

    /// <summary>Trạng thái chạy. Mặc định tất cả.</summary>
    public FlashSaleStatusFilter? Status { get; set; }

    /// <summary>Từ thời điểm.</summary>
    public DateTime? StartDate { get; set; }

    /// <summary>Đến thời điểm.</summary>
    public DateTime? EndDate { get; set; }
}

/// <summary>Lọc danh sách voucher.</summary>
public sealed class VoucherListQuery : PageQuery
{
    /// <summary><c>true</c>: chỉ voucher còn hiệu lực; <c>false</c> (mặc định): tất cả.</summary>
    public bool IsAvailable { get; set; }

    /// <summary>Từ ngày.</summary>
    public DateTime? FromDate { get; set; }

    /// <summary>Đến ngày.</summary>
    public DateTime? ToDate { get; set; }

    /// <summary>Tiêu đề.</summary>
    public string? Title { get; set; }
}

/// <summary>Lọc mã của một voucher.</summary>
public sealed class VoucherCodeListQuery : PageQuery
{
    /// <summary>
    /// <c>true</c>: liệt kê từng lần mã được dùng trong đơn; <c>false</c> (mặc định): liệt kê mã.
    /// </summary>
    public bool IsUsed { get; set; }

    /// <summary>Khi <see cref="IsUsed"/> là <c>false</c>: <c>true</c> chỉ lấy mã chưa dùng.</summary>
    public bool IsAvailable { get; set; }
}

/// <summary>Lọc danh sách chương trình chiết khấu/phụ thu.</summary>
public sealed class OrderAdjustmentProgramListQuery : PageQuery
{
    /// <summary>Từ khóa theo tên.</summary>
    public string? Key { get; set; }

    /// <summary>Trạng thái hoạt động.</summary>
    public bool? IsActive { get; set; }

    /// <summary>Loại chương trình.</summary>
    public OrderAdjustmentType? Type { get; set; }

    /// <summary>Chi nhánh áp dụng.</summary>
    public Guid? BranchId { get; set; }
}

/// <summary>Lọc danh sách đơn FnB của một chi nhánh.</summary>
public sealed class FnbOrderListQuery : PageQuery
{
    /// <summary>Chi nhánh FnB (bắt buộc).</summary>
    public Guid BranchId { get; set; }

    /// <summary>Từ thời điểm (giờ Việt Nam).</summary>
    public DateTime? FromDate { get; set; }

    /// <summary>Đến thời điểm (giờ Việt Nam).</summary>
    public DateTime? ToDate { get; set; }

    /// <summary><c>null</c> tất cả, <c>true</c> đã thanh toán, <c>false</c> chưa thanh toán.</summary>
    public bool? IsPaid { get; set; }

    /// <summary>Mã đơn hàng.</summary>
    public string? OrderCode { get; set; }

    /// <summary>Tên hoặc số điện thoại khách hàng.</summary>
    public string? KeyWord { get; set; }
}
