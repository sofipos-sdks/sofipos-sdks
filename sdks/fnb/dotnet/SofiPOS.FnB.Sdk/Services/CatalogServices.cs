using SofiPOS.FnB.Http;
using SofiPOS.FnB.Models;

namespace SofiPOS.FnB.Services;

/// <summary>Danh mục dùng chung: chi nhánh, hạng thành viên (<c>/lookups</c>).</summary>
public sealed class LookupsService : ServiceBase
{
    internal LookupsService(ApiClient api) : base(api)
    {
    }

    /// <summary>Chi nhánh của cửa hàng (gồm trạng thái hoạt động). <c>GET /lookups/branches</c></summary>
    public Task<List<Branch>> GetBranchesAsync(CancellationToken cancellationToken = default)
        => Api.GetAsync<List<Branch>>("lookups/branches", null, cancellationToken);

    /// <summary>Nhóm sản phẩm (loại PRO), dùng cho <see cref="ProductCreateRequest.CategoryId"/>. <c>GET /lookups/product-categories</c></summary>
    public Task<List<ProductCategory>> GetProductCategoriesAsync(CancellationToken cancellationToken = default)
        => Api.GetAsync<List<ProductCategory>>("lookups/product-categories", null, cancellationToken);

    /// <summary>Hạng thành viên, dùng cho <c>membershipRankIds</c>. <c>GET /lookups/membership-ranks</c></summary>
    public Task<List<MembershipRank>> GetMembershipRanksAsync(CancellationToken cancellationToken = default)
        => Api.GetAsync<List<MembershipRank>>("lookups/membership-ranks", null, cancellationToken);
}

/// <summary>Sản phẩm POS của cửa hàng (<c>/products</c>).</summary>
public sealed class ProductsService : ServiceBase
{
    internal ProductsService(ApiClient api) : base(api)
    {
    }

    /// <summary>Danh sách sản phẩm, mới cập nhật trước. <c>GET /products</c></summary>
    public Task<PagedResult<Product>> ListAsync(ProductListQuery? query = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<PagedResult<Product>>("products", Page(query).Add("keyword", query?.Keyword), cancellationToken);

    /// <summary>Duyệt toàn bộ sản phẩm qua mọi trang.</summary>
    public IAsyncEnumerable<Product> ListAllAsync(ProductListQuery? query = null, CancellationToken cancellationToken = default)
        => EnumerateAsync(query ?? new ProductListQuery(), ListAsync, cancellationToken);

    /// <summary>Chi tiết sản phẩm. <c>GET /products/{productId}</c></summary>
    public Task<Product> GetAsync(Guid productId, CancellationToken cancellationToken = default)
        => Api.GetAsync<Product>($"products/{Id(productId)}", null, cancellationToken);

    /// <summary>Đơn vị tính của sản phẩm; phần tử đầu là đơn vị cơ bản. <c>GET /products/{productId}/units</c></summary>
    public Task<List<ProductUnit>> GetUnitsAsync(Guid productId, CancellationToken cancellationToken = default)
        => Api.GetAsync<List<ProductUnit>>($"products/{Id(productId)}/units", null, cancellationToken);

    /// <summary>
    /// Tạo sản phẩm; <see cref="ProductCreateRequest.CategoryId"/> bắt buộc. Mã đã tồn tại hoặc nhóm không hợp lệ trả 422.
    /// <c>POST /products</c>
    /// </summary>
    public Task<Product> CreateAsync(ProductCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (Required(request).CategoryId == Guid.Empty)
        {
            throw new ArgumentException("Vui lòng chọn nhóm sản phẩm (CategoryId).", nameof(request));
        }

        return Api.PostAsync<Product>("products", request, cancellationToken);
    }

    /// <summary>Cập nhật sản phẩm. <c>PUT /products/{productId}</c></summary>
    public Task<Product> UpdateAsync(Guid productId, ProductUpdateRequest request, CancellationToken cancellationToken = default)
        => Api.PutAsync<Product>($"products/{Id(productId)}", Required(request), cancellationToken);

    /// <summary>Xóa sản phẩm khỏi POS (422 nếu có dữ liệu tham chiếu). <c>POST /products/{productId}/delete</c></summary>
    public Task DeleteAsync(Guid productId, CancellationToken cancellationToken = default)
        => Api.SendWithoutResultAsync(HttpMethod.Post, $"products/{Id(productId)}/delete", null, cancellationToken);

    internal static T Required<T>(T request) where T : class
        => request ?? throw new ArgumentNullException(nameof(request));
}

/// <summary>Mã QR đặt món, chỉ áp dụng chi nhánh FnB (<c>/qr-order-codes</c>).</summary>
public sealed class QrOrderCodesService : ServiceBase
{
    internal QrOrderCodesService(ApiClient api) : base(api)
    {
    }

    /// <summary>Chi nhánh FnB đang hoạt động. <c>GET /qr-order-codes/branches</c></summary>
    public Task<List<IdTitle>> GetBranchesAsync(CancellationToken cancellationToken = default)
        => Api.GetAsync<List<IdTitle>>("qr-order-codes/branches", null, cancellationToken);

    /// <summary>Kênh bán dùng được cho QR. <c>GET /qr-order-codes/sale-channels</c></summary>
    public Task<List<QrOrderSaleChannel>> GetSaleChannelsAsync(CancellationToken cancellationToken = default)
        => Api.GetAsync<List<QrOrderSaleChannel>>("qr-order-codes/sale-channels", null, cancellationToken);

    /// <summary>Bàn đang hoạt động của chi nhánh. <c>GET /qr-order-codes/tables?branchId=</c></summary>
    public Task<List<IdTitle>> GetTablesAsync(Guid branchId, CancellationToken cancellationToken = default)
        => Api.GetAsync<List<IdTitle>>("qr-order-codes/tables", new QueryString().Add("branchId", branchId), cancellationToken);

    /// <summary>Thuộc tính đơn hàng của cửa hàng. <c>GET /qr-order-codes/order-attributes</c></summary>
    public Task<List<OrderAttribute>> GetOrderAttributesAsync(CancellationToken cancellationToken = default)
        => Api.GetAsync<List<OrderAttribute>>("qr-order-codes/order-attributes", null, cancellationToken);

    /// <summary>Danh sách mã QR. <c>GET /qr-order-codes</c></summary>
    public Task<PagedResult<QrOrderCode>> ListAsync(QrOrderCodeListQuery? query = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<PagedResult<QrOrderCode>>("qr-order-codes", Page(query)
            .Add("branchId", query?.BranchId).Add("saleChannelId", query?.SaleChannelId)
            .Add("status", query?.Status).Add("keyword", query?.Keyword), cancellationToken);

    /// <summary>Duyệt toàn bộ mã QR qua mọi trang.</summary>
    public IAsyncEnumerable<QrOrderCode> ListAllAsync(QrOrderCodeListQuery? query = null, CancellationToken cancellationToken = default)
        => EnumerateAsync(query ?? new QrOrderCodeListQuery(), ListAsync, cancellationToken);

    /// <summary>Chi tiết mã QR. <c>GET /qr-order-codes/{id}</c></summary>
    public Task<QrOrderCode> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.GetAsync<QrOrderCode>($"qr-order-codes/{Id(id)}", null, cancellationToken);

    /// <summary>Tạo mã QR. <c>POST /qr-order-codes</c></summary>
    public Task<QrOrderCode> CreateAsync(QrOrderCodeRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<QrOrderCode>("qr-order-codes", ProductsService.Required(request), cancellationToken);

    /// <summary>Sửa mã QR (tính lại thời hạn từ lúc lưu); QR đã có đơn trả 422. <c>PUT /qr-order-codes/{id}</c></summary>
    public Task<QrOrderCode> UpdateAsync(Guid id, QrOrderCodeRequest request, CancellationToken cancellationToken = default)
        => Api.PutAsync<QrOrderCode>($"qr-order-codes/{Id(id)}", ProductsService.Required(request), cancellationToken);

    /// <summary>Bật/tắt mã QR (kể cả QR đã có đơn). <c>PUT /qr-order-codes/{id}/active</c></summary>
    public Task<QrOrderCode> SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
        => Api.PutAsync<QrOrderCode>($"qr-order-codes/{Id(id)}/active", new ActiveRequest(isActive), cancellationToken);

    /// <summary>Đường dẫn đặt món và ảnh QR (PNG base64) để in. <c>GET /qr-order-codes/{id}/qr</c></summary>
    public Task<QrOrderCodeImage> GetQrImageAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.GetAsync<QrOrderCodeImage>($"qr-order-codes/{Id(id)}/qr", null, cancellationToken);

    /// <summary>Xóa mã QR; QR đã có đơn trả 422. <c>POST /qr-order-codes/{id}/delete</c></summary>
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.SendWithoutResultAsync(HttpMethod.Post, $"qr-order-codes/{Id(id)}/delete", null, cancellationToken);
}

/// <summary>Khách hàng (<c>/customers</c>); không gồm chỉnh điểm.</summary>
public sealed class CustomersService : ServiceBase
{
    internal CustomersService(ApiClient api) : base(api)
    {
    }

    /// <summary>Danh sách khách hàng. <c>GET /customers</c></summary>
    public Task<PagedResult<CustomerSummary>> ListAsync(CustomerListQuery? query = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<PagedResult<CustomerSummary>>("customers", Page(query)
            .Add("key", query?.Key).Add("fromDate", query?.FromDate).Add("toDate", query?.ToDate).Add("rankId", query?.RankId), cancellationToken);

    /// <summary>Duyệt toàn bộ khách hàng qua mọi trang.</summary>
    public IAsyncEnumerable<CustomerSummary> ListAllAsync(CustomerListQuery? query = null, CancellationToken cancellationToken = default)
        => EnumerateAsync(query ?? new CustomerListQuery(), ListAsync, cancellationToken);

    /// <summary>Chi tiết khách hàng. <c>GET /customers/{id}</c></summary>
    public Task<Customer> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.GetAsync<Customer>($"customers/{Id(id)}", null, cancellationToken);

    /// <summary>Tạo khách hàng; số điện thoại trùng trả 422. <c>POST /customers</c></summary>
    public Task<Customer> CreateAsync(CustomerRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<Customer>("customers", ProductsService.Required(request), cancellationToken);

    /// <summary>Sửa khách hàng. <c>PUT /customers/{id}</c></summary>
    public Task<Customer> UpdateAsync(Guid id, CustomerRequest request, CancellationToken cancellationToken = default)
        => Api.PutAsync<Customer>($"customers/{Id(id)}", ProductsService.Required(request), cancellationToken);

    /// <summary>Xóa khách hàng (khách đã có đơn hoặc khách mặc định trả 422). <c>POST /customers/{id}/delete</c></summary>
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.SendWithoutResultAsync(HttpMethod.Post, $"customers/{Id(id)}/delete", null, cancellationToken);
}

/// <summary>Chính sách giá sản phẩm (<c>/pricing-policies</c>).</summary>
public sealed class PricingPoliciesService : ServiceBase
{
    internal PricingPoliciesService(ApiClient api) : base(api)
    {
    }

    /// <summary>Danh sách chính sách giá. <c>GET /pricing-policies</c></summary>
    public Task<PagedResult<PricingPolicySummary>> ListAsync(PricingPolicyListQuery? query = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<PagedResult<PricingPolicySummary>>("pricing-policies", Page(query)
            .Add("key", query?.Key).Add("isActive", query?.IsActive).Add("branchId", query?.BranchId), cancellationToken);

    /// <summary>Duyệt toàn bộ chính sách giá qua mọi trang.</summary>
    public IAsyncEnumerable<PricingPolicySummary> ListAllAsync(PricingPolicyListQuery? query = null, CancellationToken cancellationToken = default)
        => EnumerateAsync(query ?? new PricingPolicyListQuery(), ListAsync, cancellationToken);

    /// <summary>Chi tiết chính sách giá. <c>GET /pricing-policies/{id}</c></summary>
    public Task<PricingPolicy> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.GetAsync<PricingPolicy>($"pricing-policies/{Id(id)}", null, cancellationToken);

    /// <summary>Món kèm hợp lệ của các sản phẩm chính, dùng điền <c>addOns</c>. <c>POST /pricing-policies/product-addons</c></summary>
    public Task<List<PricingPolicyAddOnOption>> GetProductAddOnsAsync(IEnumerable<Guid> productIds, CancellationToken cancellationToken = default)
        => Api.PostAsync<List<PricingPolicyAddOnOption>>("pricing-policies/product-addons",
            new ProductIdsRequest((productIds ?? throw new ArgumentNullException(nameof(productIds))).ToList()), cancellationToken);

    /// <summary>Tạo chính sách giá. <c>POST /pricing-policies</c></summary>
    public Task<PricingPolicy> CreateAsync(PricingPolicyRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<PricingPolicy>("pricing-policies", ProductsService.Required(request), cancellationToken);

    /// <summary>Sửa chính sách giá. <c>PUT /pricing-policies/{id}</c></summary>
    public Task<PricingPolicy> UpdateAsync(Guid id, PricingPolicyRequest request, CancellationToken cancellationToken = default)
        => Api.PutAsync<PricingPolicy>($"pricing-policies/{Id(id)}", ProductsService.Required(request), cancellationToken);

    /// <summary>Xóa chính sách giá. <c>POST /pricing-policies/{id}/delete</c></summary>
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.SendWithoutResultAsync(HttpMethod.Post, $"pricing-policies/{Id(id)}/delete", null, cancellationToken);
}

/// <summary>Kênh bán của cửa hàng, ID dạng chuỗi như <c>TaiCho</c> (<c>/sale-channels</c>).</summary>
public sealed class SaleChannelsService : ServiceBase
{
    internal SaleChannelsService(ApiClient api) : base(api)
    {
    }

    /// <summary>Danh mục kênh bán hệ thống, dùng chọn <c>saleChannelId</c>. <c>GET /sale-channels/options</c></summary>
    public Task<List<SaleChannelOption>> GetOptionsAsync(CancellationToken cancellationToken = default)
        => Api.GetAsync<List<SaleChannelOption>>("sale-channels/options", null, cancellationToken);

    /// <summary>Danh sách kênh bán đã cấu hình. <c>GET /sale-channels</c></summary>
    public Task<PagedResult<SaleChannel>> ListAsync(SaleChannelListQuery? query = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<PagedResult<SaleChannel>>("sale-channels", Page(query)
            .Add("name", query?.Name).Add("isActive", query?.IsActive), cancellationToken);

    /// <summary>Duyệt toàn bộ kênh bán qua mọi trang.</summary>
    public IAsyncEnumerable<SaleChannel> ListAllAsync(SaleChannelListQuery? query = null, CancellationToken cancellationToken = default)
        => EnumerateAsync(query ?? new SaleChannelListQuery(), ListAsync, cancellationToken);

    /// <summary>Chi tiết kênh bán. <c>GET /sale-channels/{saleChannelId}</c></summary>
    public Task<SaleChannel> GetAsync(string saleChannelId, CancellationToken cancellationToken = default)
        => Api.GetAsync<SaleChannel>($"sale-channels/{Segment(saleChannelId)}", null, cancellationToken);

    /// <summary>Thêm kênh bán từ danh mục hệ thống. <c>POST /sale-channels</c></summary>
    public Task<SaleChannel> CreateAsync(SaleChannelCreateRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<SaleChannel>("sale-channels", ProductsService.Required(request), cancellationToken);

    /// <summary>Sửa kênh bán. <c>PUT /sale-channels/{saleChannelId}</c></summary>
    public Task<SaleChannel> UpdateAsync(string saleChannelId, SaleChannelUpdateRequest request, CancellationToken cancellationToken = default)
    {
        ProductsService.Required(request);
        request.SaleChannelId = saleChannelId;
        return Api.PutAsync<SaleChannel>($"sale-channels/{Segment(saleChannelId)}", request, cancellationToken);
    }

    /// <summary>Xóa kênh bán (kênh <c>TaiCho</c> hoặc kênh đã có đơn trả 422). <c>POST /sale-channels/{saleChannelId}/delete</c></summary>
    public Task DeleteAsync(string saleChannelId, CancellationToken cancellationToken = default)
        => Api.SendWithoutResultAsync(HttpMethod.Post, $"sale-channels/{Segment(saleChannelId)}/delete", null, cancellationToken);
}

/// <summary>Combo (<c>/combos</c>).</summary>
public sealed class CombosService : ServiceBase
{
    internal CombosService(ApiClient api) : base(api)
    {
    }

    /// <summary>Danh sách combo. <c>GET /combos</c></summary>
    public Task<PagedResult<ComboSummary>> ListAsync(ComboListQuery? query = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<PagedResult<ComboSummary>>("combos", Page(query).Add("key", query?.Key).Add("isActive", query?.IsActive), cancellationToken);

    /// <summary>Duyệt toàn bộ combo qua mọi trang.</summary>
    public IAsyncEnumerable<ComboSummary> ListAllAsync(ComboListQuery? query = null, CancellationToken cancellationToken = default)
        => EnumerateAsync(query ?? new ComboListQuery(), ListAsync, cancellationToken);

    /// <summary>Chi tiết combo. <c>GET /combos/{id}</c></summary>
    public Task<Combo> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.GetAsync<Combo>($"combos/{Id(id)}", null, cancellationToken);

    /// <summary>Tạo combo. <c>POST /combos</c></summary>
    public Task<Combo> CreateAsync(ComboRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<Combo>("combos", ProductsService.Required(request), cancellationToken);

    /// <summary>Sửa combo; combo đã có đơn chỉ cập nhật trạng thái. <c>PUT /combos/{id}</c></summary>
    public Task<Combo> UpdateAsync(Guid id, ComboRequest request, CancellationToken cancellationToken = default)
        => Api.PutAsync<Combo>($"combos/{Id(id)}", ProductsService.Required(request), cancellationToken);

    /// <summary>Xóa combo (đã có đơn trả 422). <c>POST /combos/{id}/delete</c></summary>
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.SendWithoutResultAsync(HttpMethod.Post, $"combos/{Id(id)}/delete", null, cancellationToken);
}

/// <summary>Flash sale (<c>/flashsales</c>).</summary>
public sealed class FlashSalesService : ServiceBase
{
    internal FlashSalesService(ApiClient api) : base(api)
    {
    }

    /// <summary>Danh sách flash sale. <c>GET /flashsales</c></summary>
    public Task<PagedResult<FlashSale>> ListAsync(FlashSaleListQuery? query = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<PagedResult<FlashSale>>("flashsales", Page(query)
            .Add("key", query?.Key).Add("isActive", query?.IsActive).Add("status", (int?)query?.Status)
            .Add("startDate", query?.StartDate).Add("endDate", query?.EndDate), cancellationToken);

    /// <summary>Duyệt toàn bộ flash sale qua mọi trang.</summary>
    public IAsyncEnumerable<FlashSale> ListAllAsync(FlashSaleListQuery? query = null, CancellationToken cancellationToken = default)
        => EnumerateAsync(query ?? new FlashSaleListQuery(), ListAsync, cancellationToken);

    /// <summary>Chi tiết flash sale. <c>GET /flashsales/{id}</c></summary>
    public Task<FlashSale> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.GetAsync<FlashSale>($"flashsales/{Id(id)}", null, cancellationToken);

    /// <summary>Tạo flash sale. <c>POST /flashsales</c></summary>
    public Task<FlashSale> CreateAsync(FlashSaleRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<FlashSale>("flashsales", ProductsService.Required(request), cancellationToken);

    /// <summary>Sửa flash sale; đã có đơn chỉ cập nhật trạng thái. <c>PUT /flashsales/{id}</c></summary>
    public Task<FlashSale> UpdateAsync(Guid id, FlashSaleRequest request, CancellationToken cancellationToken = default)
        => Api.PutAsync<FlashSale>($"flashsales/{Id(id)}", ProductsService.Required(request), cancellationToken);

    /// <summary>Xóa flash sale (đã có đơn trả 422). <c>POST /flashsales/{id}/delete</c></summary>
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.SendWithoutResultAsync(HttpMethod.Post, $"flashsales/{Id(id)}/delete", null, cancellationToken);
}

/// <summary>Voucher (<c>/vouchers</c>).</summary>
public sealed class VouchersService : ServiceBase
{
    internal VouchersService(ApiClient api) : base(api)
    {
    }

    /// <summary>Danh sách voucher. <c>GET /vouchers</c></summary>
    public Task<PagedResult<VoucherSummary>> ListAsync(VoucherListQuery? query = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<PagedResult<VoucherSummary>>("vouchers", Page(query)
            .Add("isAvailable", query?.IsAvailable).Add("fromDate", query?.FromDate).Add("toDate", query?.ToDate)
            .Add("title", query?.Title), cancellationToken);

    /// <summary>Duyệt toàn bộ voucher qua mọi trang.</summary>
    public IAsyncEnumerable<VoucherSummary> ListAllAsync(VoucherListQuery? query = null, CancellationToken cancellationToken = default)
        => EnumerateAsync(query ?? new VoucherListQuery(), ListAsync, cancellationToken);

    /// <summary>Chi tiết voucher. <c>GET /vouchers/{id}</c></summary>
    public Task<Voucher> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.GetAsync<Voucher>($"vouchers/{Id(id)}", null, cancellationToken);

    /// <summary>Mã của voucher hoặc lịch sử dùng mã. <c>GET /vouchers/{id}/codes</c></summary>
    public Task<PagedResult<VoucherCode>> GetCodesAsync(Guid id, VoucherCodeListQuery? query = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<PagedResult<VoucherCode>>($"vouchers/{Id(id)}/codes", Page(query)
            .Add("isUsed", query?.IsUsed).Add("isAvailable", query?.IsAvailable), cancellationToken);

    /// <summary>Tạo voucher (luôn chưa hoạt động). <c>POST /vouchers</c></summary>
    public Task<Voucher> CreateAsync(VoucherRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<Voucher>("vouchers", ProductsService.Required(request), cancellationToken);

    /// <summary>Sửa voucher; voucher đã có mã được dùng trả 422. <c>PUT /vouchers/{id}</c></summary>
    public Task<Voucher> UpdateAsync(Guid id, VoucherRequest request, CancellationToken cancellationToken = default)
        => Api.PutAsync<Voucher>($"vouchers/{Id(id)}", ProductsService.Required(request), cancellationToken);

    /// <summary>Bật/tắt voucher. <c>PUT /vouchers/{id}/active</c></summary>
    public Task<Voucher> SetActiveAsync(Guid id, bool isActive, CancellationToken cancellationToken = default)
        => Api.PutAsync<Voucher>($"vouchers/{Id(id)}/active", new ActiveRequest(isActive), cancellationToken);

    /// <summary>Xóa voucher (đang hoạt động hoặc đã dùng trả 422). <c>POST /vouchers/{id}/delete</c></summary>
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.SendWithoutResultAsync(HttpMethod.Post, $"vouchers/{Id(id)}/delete", null, cancellationToken);
}

/// <summary>Chương trình chiết khấu/phụ thu theo đơn (<c>/order-adjustment-programs</c>).</summary>
public sealed class OrderAdjustmentProgramsService : ServiceBase
{
    internal OrderAdjustmentProgramsService(ApiClient api) : base(api)
    {
    }

    /// <summary>Danh sách chương trình. <c>GET /order-adjustment-programs</c></summary>
    public Task<PagedResult<OrderAdjustmentProgramSummary>> ListAsync(OrderAdjustmentProgramListQuery? query = null, CancellationToken cancellationToken = default)
        => Api.GetAsync<PagedResult<OrderAdjustmentProgramSummary>>("order-adjustment-programs", Page(query)
            .Add("key", query?.Key).Add("isActive", query?.IsActive).Add("type", (int?)query?.Type)
            .Add("branchId", query?.BranchId), cancellationToken);

    /// <summary>Duyệt toàn bộ chương trình qua mọi trang.</summary>
    public IAsyncEnumerable<OrderAdjustmentProgramSummary> ListAllAsync(OrderAdjustmentProgramListQuery? query = null, CancellationToken cancellationToken = default)
        => EnumerateAsync(query ?? new OrderAdjustmentProgramListQuery(), ListAsync, cancellationToken);

    /// <summary>Chi tiết chương trình (kèm <c>Version</c>). <c>GET /order-adjustment-programs/{id}</c></summary>
    public Task<OrderAdjustmentProgram> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.GetAsync<OrderAdjustmentProgram>($"order-adjustment-programs/{Id(id)}", null, cancellationToken);

    /// <summary>Tạo chương trình. <c>POST /order-adjustment-programs</c></summary>
    public Task<OrderAdjustmentProgram> CreateAsync(OrderAdjustmentProgramRequest request, CancellationToken cancellationToken = default)
        => Api.PostAsync<OrderAdjustmentProgram>("order-adjustment-programs", ProductsService.Required(request), cancellationToken);

    /// <summary>Sửa chương trình; gửi <c>Version</c> vừa đọc để chống ghi đè. <c>PUT /order-adjustment-programs/{id}</c></summary>
    public Task<OrderAdjustmentProgram> UpdateAsync(Guid id, OrderAdjustmentProgramRequest request, CancellationToken cancellationToken = default)
        => Api.PutAsync<OrderAdjustmentProgram>($"order-adjustment-programs/{Id(id)}", ProductsService.Required(request), cancellationToken);

    /// <summary>Xóa chương trình. <c>POST /order-adjustment-programs/{id}/delete</c></summary>
    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => Api.SendWithoutResultAsync(HttpMethod.Post, $"order-adjustment-programs/{Id(id)}/delete", null, cancellationToken);
}

internal sealed record ActiveRequest([property: System.Text.Json.Serialization.JsonPropertyName("isActive")] bool IsActive);

internal sealed record ProductIdsRequest([property: System.Text.Json.Serialization.JsonPropertyName("productIds")] List<Guid> ProductIds);
