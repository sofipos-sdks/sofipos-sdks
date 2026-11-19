using MongoDB.Driver;

namespace SofiPOS.FnB.Demo.Services;

public sealed class DemoWorker(DemoStore store, SyncService sync, ILogger<DemoWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while(!stoppingToken.IsCancellationRequested)
        {
            try { await store.EnsureIndexesAsync(stoppingToken); break; }
            catch(OperationCanceledException) when(stoppingToken.IsCancellationRequested) { return; }
            catch { logger.LogWarning("MongoDB chưa sẵn sàng. Worker sẽ thử kết nối lại sau 10 giây."); }
            await Task.Delay(TimeSpan.FromSeconds(10),stoppingToken);
        }
        await Task.WhenAll(LoopAsync(ProcessWebhookAsync,stoppingToken),LoopAsync(ProcessSyncAsync,stoppingToken));
    }

    private static async Task LoopAsync(Func<CancellationToken,Task<bool>> next,CancellationToken ct)
    {
        while(!ct.IsCancellationRequested)
        {
            try { if(await next(ct))continue; }
            catch(OperationCanceledException) when(ct.IsCancellationRequested){return;}
            catch { /* Mất MongoDB: giữ công việc trong DB, lấy lại khi kết nối phục hồi. */ }
            await Task.Delay(TimeSpan.FromSeconds(2),ct);
        }
    }

    private async Task<bool> ProcessWebhookAsync(CancellationToken ct)
    {
        var now=DateTime.UtcNow;
        await store.Webhooks.UpdateManyAsync(x=>x.State=="running" && x.LeaseUntil<now && x.Attempts>=3,
            Builders<WebhookRecord>.Update.Set(x=>x.State,"failed").Set(x=>x.Error,"Worker đã dừng sau lần thử cuối. Có thể xử lý lại thủ công."),cancellationToken:ct);
        var filter=Builders<WebhookRecord>.Filter.And(
            Builders<WebhookRecord>.Filter.Eq(x=>x.SignatureStatus,"valid"),Builders<WebhookRecord>.Filter.Lt(x=>x.Attempts,3),
            Builders<WebhookRecord>.Filter.Or(Builders<WebhookRecord>.Filter.Eq(x=>x.State,"pending"),Builders<WebhookRecord>.Filter.And(Builders<WebhookRecord>.Filter.Eq(x=>x.State,"running"),Builders<WebhookRecord>.Filter.Lt(x=>x.LeaseUntil,now))));
        var record=await store.Webhooks.FindOneAndUpdateAsync(filter,
            Builders<WebhookRecord>.Update.Set(x=>x.State,"running").Set(x=>x.LeaseUntil,now.AddMinutes(5)).Inc(x=>x.Attempts,1),
            new FindOneAndUpdateOptions<WebhookRecord>{ReturnDocument=ReturnDocument.After,Sort=Builders<WebhookRecord>.Sort.Ascending(x=>x.CreatedAt)},ct);
        if(record==null)return false;
        try
        {
            await sync.ProcessWebhookAsync(record,ct);
            if(record.State!="ignored")record.State="processed";
            record.Error=null;
        }
        catch(OperationCanceledException) when(ct.IsCancellationRequested){throw;}
        catch(Exception error)
        {
            record.Error=SyncService.PublicError(error);
            record.State="failed";
        }
        record.LeaseUntil=null;
        await store.Webhooks.ReplaceOneAsync(x=>x.Id==record.Id,record,cancellationToken:ct);
        return true;
    }

    private async Task<bool> ProcessSyncAsync(CancellationToken ct)
    {
        var now=DateTime.UtcNow;
        var filter=Builders<SyncRun>.Filter.Or(Builders<SyncRun>.Filter.Eq(x=>x.State,"pending"),Builders<SyncRun>.Filter.And(Builders<SyncRun>.Filter.Eq(x=>x.State,"running"),Builders<SyncRun>.Filter.Lt(x=>x.LeaseUntil,now)));
        var run=await store.SyncRuns.FindOneAndUpdateAsync(filter,
            Builders<SyncRun>.Update.Set(x=>x.State,"running").Set(x=>x.LeaseUntil,now.AddMinutes(5)),
            new FindOneAndUpdateOptions<SyncRun>{ReturnDocument=ReturnDocument.After,Sort=Builders<SyncRun>.Sort.Ascending(x=>x.CreatedAt)},ct);
        if(run==null)return false;
        run.Completed=0;run.Errors.Clear();
        try { await sync.RunAsync(run,ct); }
        catch(OperationCanceledException) when(ct.IsCancellationRequested){throw;}
        catch(Exception error)
        {
            run.State="failed";run.Errors.Add(SyncService.PublicError(error));run.LeaseUntil=null;run.UpdatedAt=DateTime.UtcNow;
            await store.SyncRuns.ReplaceOneAsync(x=>x.Id==run.Id,run,cancellationToken:ct);
        }
        return true;
    }
}
