namespace SofiPOS.FnB.Models;

/// <summary>Quy tắc làm tròn điểm tích lũy.</summary>
public enum PointEarnRoundingRule
{
    /// <summary>Làm tròn xuống.</summary>
    RoundDown = 1,

    /// <summary>Làm tròn lên.</summary>
    RoundUp = 2,

    /// <summary>Làm tròn gần nhất.</summary>
    RoundNearest = 3,
}

/// <summary>Trừ tiền điểm thành viên trên giá trị trước hay sau VAT.</summary>
public enum PointBurnTaxBasis
{
    /// <summary>Trước VAT.</summary>
    BeforeVat = 1,

    /// <summary>Sau VAT.</summary>
    AfterVat = 2,
}

/// <summary>Loại combo.</summary>
public enum ComboType
{
    /// <summary>Combo cố định (dùng <c>FixedProducts</c>).</summary>
    Fixed = 0,

    /// <summary>Combo tự chọn (dùng <c>Groups</c>).</summary>
    Mix = 1,
}

/// <summary>Cách tính giá combo.</summary>
public enum ComboPricingMode
{
    /// <summary>Giá chung của combo.</summary>
    Common = 0,

    /// <summary>Tổng giá các sản phẩm.</summary>
    SumOfProducts = 1,
}

/// <summary>Kiểu giá trị giảm/phụ thu.</summary>
public enum DiscountType
{
    /// <summary>Không áp dụng.</summary>
    None = 0,

    /// <summary>Theo phần trăm.</summary>
    Percent = 1,

    /// <summary>Theo số tiền.</summary>
    Amount = 2,
}

/// <summary>Loại kênh bán.</summary>
public enum SaleChannelECommerceType
{
    /// <summary>Bán trực tiếp.</summary>
    Offline = 0,

    /// <summary>Thương mại điện tử có thanh toán.</summary>
    ECommerceWithPayment = 1,

    /// <summary>Thương mại điện tử không thanh toán.</summary>
    ECommerceWithoutPayment = 2,
}

/// <summary>Loại chương trình theo đơn.</summary>
public enum OrderAdjustmentType
{
    /// <summary>Chiết khấu.</summary>
    Discount = 1,

    /// <summary>Phụ thu.</summary>
    Surcharge = 2,
}

/// <summary>Kiểu in của món.</summary>
public enum ProductPrintType
{
    /// <summary>Không in.</summary>
    None = 0,

    /// <summary>In tem.</summary>
    Label = 1,

    /// <summary>In phiếu chế biến.</summary>
    Kitchen = 2,
}

/// <summary>Loại sản phẩm.</summary>
public enum ProductType
{
    /// <summary>Hàng hóa.</summary>
    Goods = 0,

    /// <summary>Dịch vụ.</summary>
    Service = 1,

    /// <summary>Thành phẩm.</summary>
    FinishedGoods = 2,

    /// <summary>Bán thành phẩm.</summary>
    SemiFinishedGoods = 3,

    /// <summary>Công cụ dụng cụ.</summary>
    Tool = 4,

    /// <summary>Nguyên vật liệu.</summary>
    Material = 5,

    /// <summary>Tài sản cố định.</summary>
    FixedAsset = 6,
}

/// <summary>Phương thức thanh toán.</summary>
public enum PaymentMethod
{
    /// <summary>Tiền mặt.</summary>
    Cash = 1,

    /// <summary>Chuyển khoản/ngân hàng.</summary>
    Bank = 2,
}

/// <summary>Bên trả phí giao hàng.</summary>
public enum ShippingFeePayer
{
    /// <summary>Khách hàng.</summary>
    Customer = 1,

    /// <summary>Cửa hàng.</summary>
    Merchant = 2,
}

/// <summary>Trạng thái dòng món/combo trong đơn.</summary>
public enum OrderLineStatus
{
    /// <summary>Đang hoạt động.</summary>
    Active = 1,

    /// <summary>Đã hủy.</summary>
    Cancelled = 2,

    /// <summary>Đã trả hàng.</summary>
    Returned = 3,
}
