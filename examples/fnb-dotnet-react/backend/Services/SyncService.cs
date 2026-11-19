using System.Globalization;
using System.Text.Json;
using MongoDB.Driver;
using SofiPOS.FnB.Models;
using SofiPOS.FnB.Http;
using SofiPOS.FnB.Exceptions;
using Client = SofiPOS.FnB.SofiPosFnBClient;

namespace SofiPOS.FnB.Demo.Services;

public sealed class SyncService(SdkGateway gateway, DemoStore store, ConnectionService connections)
{
    public static readonly string[] Resources = ["products", "customers", "combos", "flashsales", "pricing-policies", "vouchers", "order-adjustment-programs", "sale-channels", "qr-order-codes", "branches", "membership-ranks", "menu", "areas", "payment-options", "orders"];

    public async Task RunAsync(SyncRun run, CancellationToken ct)
    {
        foreach (var resource in run.Input.Resources)
        {
            await EnsureConnectionAsync(run.ConnectionId, ct);
            run.CurrentResource = resource;
            await ProgressAsync(run, ct);
            try { await SyncResourceAsync(resource, run, ct); }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
            catch (Exception e) { run.Errors.Add($"{resource}: {PublicError(e)}"); }
            run.Completed++;
            await ProgressAsync(run, ct);
        }
        run.State = run.Errors.Count == 0 ? "completed" : "completed-with-errors";
        run.LeaseUntil = null;
        await ProgressAsync(run, ct);
    }

    private async Task SyncResourceAsync(string resource, SyncRun run, CancellationToken ct)
    {
        switch (resource)
        {
            case "products": await CatalogAsync(run, "Products", (s,t) => s.Products.ListAllAsync(new ProductListQuery { PageSize = 100 },t), async (s,id,t) => await s.Products.GetAsync(Guid.Parse(id),t), ct); break;
            case "customers": await CatalogAsync(run, "Customers", (s,t) => s.Customers.ListAllAsync(new CustomerListQuery { PageSize = 100 },t), async (s,id,t) => await s.Customers.GetAsync(Guid.Parse(id),t), ct); break;
            case "combos": await CatalogAsync(run, "Combos", (s,t) => s.Combos.ListAllAsync(new ComboListQuery { PageSize = 100 },t), async (s,id,t) => await s.Combos.GetAsync(Guid.Parse(id),t), ct); break;
            case "flashsales": await CatalogAsync(run, "FlashSales", (s,t) => s.FlashSales.ListAllAsync(new FlashSaleListQuery { PageSize = 100 },t), async (s,id,t) => await s.FlashSales.GetAsync(Guid.Parse(id),t), ct); break;
            case "pricing-policies": await CatalogAsync(run, "PricingPolicies", (s,t) => s.PricingPolicies.ListAllAsync(new PricingPolicyListQuery { PageSize = 100 },t), async (s,id,t) => await s.PricingPolicies.GetAsync(Guid.Parse(id),t), ct); break;
            case "vouchers": await CatalogAsync(run, "Vouchers", (s,t) => s.Vouchers.ListAllAsync(new VoucherListQuery { PageSize = 100 },t), async (s,id,t) => await s.Vouchers.GetAsync(Guid.Parse(id),t), ct); break;
            case "order-adjustment-programs": await CatalogAsync(run, "OrderAdjustmentPrograms", (s,t) => s.OrderAdjustmentPrograms.ListAllAsync(new OrderAdjustmentProgramListQuery { PageSize = 100 },t), async (s,id,t) => await s.OrderAdjustmentPrograms.GetAsync(Guid.Parse(id),t), ct); break;
            case "sale-channels": await CatalogAsync(run, "SaleChannels", (s,t) => s.SaleChannels.ListAllAsync(new SaleChannelListQuery { PageSize = 100 },t), async (s,id,t) => await s.SaleChannels.GetAsync(id,t), ct); break;
            case "qr-order-codes": await CatalogAsync(run, "QrOrderCodes", (s,t) => s.QrOrderCodes.ListAllAsync(new QrOrderCodeListQuery { PageSize = 100 },t), async (s,id,t) => await s.QrOrderCodes.GetAsync(Guid.Parse(id),t), ct); break;
            case "orders":
                await CatalogAsync(run, "FnbOrders", (s,t) => s.FnbOrders.ListAllAsync(new FnbOrderListQuery
                {
                    PageSize = 100, BranchId = Branch(run.Input.BranchId),
                    FromDate = Date(run.Input.FromDate), ToDate = Date(run.Input.ToDate)
                },t), async (s,id,t) => await s.FnbOrders.GetAsync(Guid.Parse(id),t), ct); break;
            case "branches": await gateway.CallAsync("Lookups.GetBranchesAsync", (s,t) => s.Lookups.GetBranchesAsync(t), source: "sync", ct:ct); break;
            case "membership-ranks": await gateway.CallAsync("Lookups.GetMembershipRanksAsync", (s,t) => s.Lookups.GetMembershipRanksAsync(t), source: "sync", ct:ct); break;
            case "menu": await gateway.CallAsync("Fnb.GetMenuAsync", (s,t) => s.Fnb.GetMenuAsync(Branch(run.Input.BranchId),run.Input.SaleChannelId,t), context: run.Input.BranchId+"/"+run.Input.SaleChannelId, source:"sync",ct:ct); break;
            case "areas": await gateway.CallAsync("Fnb.GetAreasAsync", (s,t) => s.Fnb.GetAreasAsync(Branch(run.Input.BranchId),t), context: run.Input.BranchId, source:"sync",ct:ct); break;
            case "payment-options": await gateway.CallAsync("Fnb.GetPaymentOptionsAsync", (s,t) => s.Fnb.GetPaymentOptionsAsync(Branch(run.Input.BranchId),t), context: run.Input.BranchId, source:"sync",ct:ct); break;
        }
    }

    private async Task CatalogAsync<T>(SyncRun run, string service, Func<Client,CancellationToken,IAsyncEnumerable<T>> enumerate,
        Func<Client,string,CancellationToken,Task<object>> get, CancellationToken ct)
    {
        await gateway.EnumerateAsync(service+".ListAllAsync", enumerate, async (row,token) =>
        {
            await EnsureConnectionAsync(run.ConnectionId, token);
            var json = JsonSerializer.SerializeToElement(row, SofiPosJson.Options);
            var id = Id(json);
            await gateway.CallAsync(service+".GetAsync", (sdk,t) => get(sdk,id,t), id:id, source:"sync",ct:token);
            if (service == "Vouchers")
            {
                var page = 1;
                while (true)
                {
                    var codes = await gateway.CallAsync("Vouchers.GetCodesAsync", (s,t) => s.Vouchers.GetCodesAsync(Guid.Parse(id),new VoucherCodeListQuery { PageNumber=page,PageSize=100 },t), id:id,context:page.ToString(CultureInfo.InvariantCulture),source:"sync",ct:token);
                    if (!codes.Data.MetaData.HasNext || codes.Data.Items.Count == 0) break;
                    page++;
                }
            }
            run.Records++;
            await ProgressAsync(run, token);
        }, ct);
    }

    public async Task ProcessWebhookAsync(WebhookRecord record, CancellationToken ct)
    {
        await EnsureConnectionAsync(record.ConnectionId, ct);
        var root = JsonDocument.Parse(record.PayloadJson).RootElement;
        var data = root.GetProperty("data");
        if (record.Event == "ping") return;
        if (record.Event is "order.created" or "order.updated")
        {
            var id = data.GetProperty("orderId").GetString()!;
            try { await gateway.CallAsync("FnbOrders.GetAsync", (s,t) => s.FnbOrders.GetAsync(Guid.Parse(id),t), id:id,source:"webhook",ct:ct); }
            catch (SofiPosApiException e) when ((int)e.StatusCode == 404) { await store.DeleteSnapshotAsync(record.ConnectionId,"orders",id,ct); }
            await RefreshAreasAsync(ct);
            return;
        }
        var catalog = record.Event switch
        {
            "product.changed" => "Products", "customer.changed" => "Customers", "combo.changed" => "Combos",
            "flash_sale.changed" => "FlashSales", "pricing_policy.changed" => "PricingPolicies", "voucher.changed" => "Vouchers",
            "sale_channel.changed" => "SaleChannels", "qr_order_code.changed" => "QrOrderCodes", "order_adjustment_program.changed" => "OrderAdjustmentPrograms", _ => ""
        };
        if (!string.IsNullOrEmpty(catalog))
        {
            if (data.TryGetProperty("deletedIds",out var deleted))
                foreach (var id in deleted.EnumerateArray()) await store.DeleteSnapshotAsync(record.ConnectionId,SdkGateway.ResourceFor(catalog),id.GetString()!,ct);
            if (data.TryGetProperty("ids",out var ids))
                foreach (var node in ids.EnumerateArray())
                {
                    var id=node.GetString()!;
                    try
                    {
                        await GetCatalogRecordAsync(catalog,id,ct);
                        if(catalog=="Products")
                            await gateway.CallAsync("Products.GetUnitsAsync",(s,t)=>s.Products.GetUnitsAsync(Guid.Parse(id),t),id:id,source:"webhook",ct:ct);
                        if(catalog=="Vouchers")await RefreshVoucherCodesAsync(id,ct);
                    }
                    catch (SofiPosApiException e) when ((int)e.StatusCode == 404) { await store.DeleteSnapshotAsync(record.ConnectionId,SdkGateway.ResourceFor(catalog),id,ct); }
                }
            if (catalog is "Products" or "Combos" or "FlashSales" or "PricingPolicies" or "SaleChannels") await RefreshMenuAsync(ct);
            return;
        }
        if (record.Event == "membership_rank.changed") { await gateway.CallAsync("Lookups.GetMembershipRanksAsync",(s,t)=>s.Lookups.GetMembershipRanksAsync(t),source:"webhook",ct:ct); return; }
        if (record.Event == "category.changed") { await RefreshMenuAsync(ct); return; }
        if (record.Event == "area_table.changed")
        {
            await RefreshAreasAsync(ct);
            return;
        }
        record.State="ignored";
    }

    private Task<SdkResult<object>> GetCatalogRecordAsync(string service,string id,CancellationToken ct)
        => gateway.CallAsync<object>(service+".GetAsync",async (s,t) => service switch
        {
            "Products" => await s.Products.GetAsync(Guid.Parse(id),t), "Customers" => await s.Customers.GetAsync(Guid.Parse(id),t),
            "Combos" => await s.Combos.GetAsync(Guid.Parse(id),t), "FlashSales" => await s.FlashSales.GetAsync(Guid.Parse(id),t),
            "PricingPolicies" => await s.PricingPolicies.GetAsync(Guid.Parse(id),t), "Vouchers" => await s.Vouchers.GetAsync(Guid.Parse(id),t),
            "SaleChannels" => await s.SaleChannels.GetAsync(id,t), "QrOrderCodes" => await s.QrOrderCodes.GetAsync(Guid.Parse(id),t),
            "OrderAdjustmentPrograms" => await s.OrderAdjustmentPrograms.GetAsync(Guid.Parse(id),t), _ => throw new ArgumentException("Nhóm dữ liệu không hợp lệ.")
        },id:id,source:"webhook",ct:ct);

    private async Task RefreshMenuAsync(CancellationToken ct)
    {
        var c=await connections.RequiredAsync(ct);
        var cached=await store.Snapshots.Find(x=>x.ConnectionId==c.ConnectionId && x.Resource=="menu").ToListAsync(ct);
        var contexts=cached.Select(x=>x.Context).Append((c.DefaultBranchId??"default")+"/"+c.DefaultSaleChannelId).Distinct();
        foreach(var context in contexts)
        {
            var parts=context.Split('/');
            await gateway.CallAsync("Fnb.GetMenuAsync",(s,t)=>s.Fnb.GetMenuAsync(Branch(parts[0]),parts.Length>1?parts[1]:"TaiCho",t),context:context,source:"webhook",ct:ct);
        }
    }

    private async Task RefreshAreasAsync(CancellationToken ct)
    {
        var c=await connections.RequiredAsync(ct);
        var cached=await store.Snapshots.Find(x=>x.ConnectionId==c.ConnectionId && x.Resource=="areas").ToListAsync(ct);
        foreach(var branch in cached.Select(x=>x.Context.Split('/')[0]).Append(c.DefaultBranchId??"default").Distinct())
            await gateway.CallAsync("Fnb.GetAreasAsync",(s,t)=>s.Fnb.GetAreasAsync(Branch(branch),t),context:branch,source:"webhook",ct:ct);
    }

    private async Task RefreshVoucherCodesAsync(string id,CancellationToken ct)
    {
        for(var page=1;;page++)
        {
            var response=await gateway.CallAsync("Vouchers.GetCodesAsync",(s,t)=>s.Vouchers.GetCodesAsync(Guid.Parse(id),new VoucherCodeListQuery{PageNumber=page,PageSize=100},t),id:id,context:page.ToString(CultureInfo.InvariantCulture),source:"webhook",ct:ct);
            if(!response.Data.MetaData.HasNext || response.Data.Items.Count==0)break;
        }
    }

    private async Task EnsureConnectionAsync(string id,CancellationToken ct)
    {
        if ((await connections.RequiredAsync(ct)).ConnectionId!=id) throw new InvalidOperationException("Kết nối đã thay đổi. Hãy tạo phiên đồng bộ mới.");
    }
    private Task ProgressAsync(SyncRun run,CancellationToken ct)
    {
        run.UpdatedAt=DateTime.UtcNow;
        if(run.State=="running")run.LeaseUntil=DateTime.UtcNow.AddMinutes(5);
        return store.SyncRuns.ReplaceOneAsync(x=>x.Id==run.Id,run,cancellationToken:ct);
    }
    private static Guid? Branch(string? value)=>Guid.TryParse(value,out var id)?id:null;
    private static DateTime? Date(string? value)=>string.IsNullOrEmpty(value)?null:VietnamDateTimeConverter.Parse(value);
    private static string Id(JsonElement row)
    {
        foreach(var name in new[]{"id","productId","orderId","saleChannelId"}) if(row.TryGetProperty(name,out var id)&&id.ValueKind==JsonValueKind.String)return id.GetString()!;
        throw new InvalidOperationException("API danh sách không có ID bản ghi.");
    }
    public static string PublicError(Exception e)=>e is SofiPosApiException sdk?$"HTTP {(int)sdk.StatusCode}: {sdk.Message}":e is MongoException?"MongoDB chưa sẵn sàng.":e is InvalidOperationException or ArgumentException?e.Message:"Không lấy được dữ liệu. Kiểm tra kết nối và lịch sử SDK.";
}
