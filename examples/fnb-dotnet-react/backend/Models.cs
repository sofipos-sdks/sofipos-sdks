using MongoDB.Bson.Serialization.Attributes;

namespace SofiPOS.FnB.Demo;

public sealed class ConnectionRecord
{
    [BsonId] public string Id { get; set; } = "current";
    public string ConnectionId { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "Cửa hàng thử nghiệm";
    public string BaseUrl { get; set; } = "";
    public string ProtectedApiKey { get; set; } = "";
    public string ProtectedWebhookSecret { get; set; } = "";
    public string? DefaultBranchId { get; set; }
    public string DefaultSaleChannelId { get; set; } = "TaiCho";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public sealed class ConnectionInput
{
    public string Name { get; set; } = "Cửa hàng thử nghiệm";
    public string BaseUrl { get; set; } = "";
    public string? ApiKey { get; set; }
    public string? WebhookSecret { get; set; }
    public bool ClearWebhookSecret { get; set; }
    public string? DefaultBranchId { get; set; }
    public string DefaultSaleChannelId { get; set; } = "TaiCho";
}

public sealed class Snapshot
{
    [BsonId] public string Id { get; set; } = "";
    public string ConnectionId { get; set; } = "";
    public string Resource { get; set; } = "";
    public string ResourceId { get; set; } = "";
    public string Context { get; set; } = "";
    public string PayloadJson { get; set; } = "{}";
    public bool IsSummary { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public sealed class ApiCall
{
    [BsonId] public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string ConnectionId { get; set; } = "";
    public string Operation { get; set; } = "";
    public string Source { get; set; } = "manual";
    public string CallId { get; set; } = "";
    public string Method { get; set; } = "";
    public string Url { get; set; } = "";
    public int? StatusCode { get; set; }
    public long DurationMs { get; set; }
    public string? TraceId { get; set; }
    public string? Error { get; set; }
    public string? RequestJson { get; set; }
    public string? ResponseJson { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public sealed class WebhookRecord
{
    [BsonId] public string Id { get; set; } = "";
    public string ConnectionId { get; set; } = "";
    public string EventId { get; set; } = "";
    public string Event { get; set; } = "";
    public string PayloadJson { get; set; } = "{}";
    public string SignatureStatus { get; set; } = "valid";
    public string Source { get; set; } = "pos";
    public Dictionary<string, string> Headers { get; set; } = new();
    public string State { get; set; } = "pending";
    public int ReceivedCount { get; set; } = 1;
    public int Attempts { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LeaseUntil { get; set; }
}

public sealed class SyncInput
{
    public List<string> Resources { get; set; } = new();
    public string? BranchId { get; set; }
    public string SaleChannelId { get; set; } = "TaiCho";
    public string? FromDate { get; set; }
    public string? ToDate { get; set; }
}

public sealed class SyncRun
{
    [BsonId] public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string ConnectionId { get; set; } = "";
    public SyncInput Input { get; set; } = new();
    public string State { get; set; } = "pending";
    public string CurrentResource { get; set; } = "";
    public int Completed { get; set; }
    public int Records { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LeaseUntil { get; set; }
}

public sealed class WebhookSimulation
{
    public string Event { get; set; } = "ping";
    public string SignatureMode { get; set; } = "valid";
    public string? EventId { get; set; }
    public string? OrderId { get; set; }
    public string? Code { get; set; }
    public string? BranchId { get; set; }
    public List<string> Ids { get; set; } = new();
    public List<string> DeletedIds { get; set; } = new();
}
