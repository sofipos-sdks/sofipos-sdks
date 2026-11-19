using MongoDB.Bson;
using MongoDB.Driver;

namespace SofiPOS.FnB.Demo;

public sealed class DemoStore
{
    private readonly IMongoDatabase _db;
    public IMongoCollection<ConnectionRecord> Connections => _db.GetCollection<ConnectionRecord>("connections");
    public IMongoCollection<Snapshot> Snapshots => _db.GetCollection<Snapshot>("snapshots");
    public IMongoCollection<ApiCall> Calls => _db.GetCollection<ApiCall>("api_calls");
    public IMongoCollection<WebhookRecord> Webhooks => _db.GetCollection<WebhookRecord>("webhook_events");
    public IMongoCollection<SyncRun> SyncRuns => _db.GetCollection<SyncRun>("sync_runs");

    public DemoStore(IConfiguration configuration)
    {
        var settings = MongoClientSettings.FromConnectionString(configuration["Mongo:ConnectionString"] ?? "mongodb://127.0.0.1:27017");
        settings.ServerSelectionTimeout = TimeSpan.FromSeconds(3);
        settings.ConnectTimeout = TimeSpan.FromSeconds(3);
        _db = new MongoClient(settings).GetDatabase(configuration["Mongo:Database"] ?? "sofipos_sdk_demo");
    }

    public Task PingAsync(CancellationToken ct) => _db.RunCommandAsync<BsonDocument>(new BsonDocument("ping", 1), cancellationToken: ct);

    public async Task EnsureIndexesAsync(CancellationToken ct)
    {
        await Calls.Indexes.CreateOneAsync(new CreateIndexModel<ApiCall>(Builders<ApiCall>.IndexKeys.Ascending(x => x.ConnectionId).Descending(x => x.CreatedAt)), cancellationToken: ct);
        await Snapshots.Indexes.CreateOneAsync(new CreateIndexModel<Snapshot>(Builders<Snapshot>.IndexKeys.Ascending(x => x.ConnectionId).Ascending(x => x.Resource).Descending(x => x.UpdatedAt)), cancellationToken: ct);
        await Webhooks.Indexes.CreateOneAsync(new CreateIndexModel<WebhookRecord>(Builders<WebhookRecord>.IndexKeys.Ascending(x => x.ConnectionId).Ascending(x => x.EventId), new CreateIndexOptions { Unique = true }), cancellationToken: ct);
        await Webhooks.Indexes.CreateOneAsync(new CreateIndexModel<WebhookRecord>(Builders<WebhookRecord>.IndexKeys.Ascending(x => x.State).Ascending(x => x.CreatedAt)), cancellationToken: ct);
        await SyncRuns.Indexes.CreateOneAsync(new CreateIndexModel<SyncRun>(Builders<SyncRun>.IndexKeys.Ascending(x => x.State).Ascending(x => x.CreatedAt)), cancellationToken: ct);
    }

    public async Task SaveSnapshotAsync(string connection, string resource, string id, string context, string json, bool summary, CancellationToken ct)
    {
        var key = $"{connection}:{resource}:{context}:{id}";
        if (summary)
        {
            var insert = new Snapshot { Id = key, ConnectionId = connection, Resource = resource, ResourceId = id, Context = context, PayloadJson = json, IsSummary = true };
            // Một trang danh sách không được ghi đè bản chi tiết đã đọc trước đó.
            var update = Builders<Snapshot>.Update.SetOnInsert(x => x.ConnectionId, insert.ConnectionId)
                .SetOnInsert(x => x.Resource, resource).SetOnInsert(x => x.ResourceId, id).SetOnInsert(x => x.Context, context)
                .SetOnInsert(x => x.PayloadJson, json).SetOnInsert(x => x.IsSummary, true).SetOnInsert(x => x.UpdatedAt, insert.UpdatedAt);
            await Snapshots.UpdateOneAsync(x => x.Id == key, update, new UpdateOptions { IsUpsert = true }, ct);
        }
        else
        {
            await Snapshots.ReplaceOneAsync(x => x.Id == key, new Snapshot { Id = key, ConnectionId = connection, Resource = resource, ResourceId = id, Context = context, PayloadJson = json }, new ReplaceOptions { IsUpsert = true }, ct);
        }
    }

    public Task DeleteSnapshotAsync(string connection, string resource, string id, CancellationToken ct)
        => Snapshots.DeleteManyAsync(x => x.ConnectionId == connection && x.Resource == resource && x.ResourceId == id, ct);
}
