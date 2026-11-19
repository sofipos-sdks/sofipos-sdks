namespace SofiPOS.FnB.Demo;

// Thông tin xác thực chỉ nằm trong RAM backend, không được trả về frontend.
public sealed class ConnectionRecord
{
    public string ConnectionId { get; set; } = Guid.NewGuid().ToString("N");
    public string Name { get; set; } = "Cửa hàng thử nghiệm";
    public string BaseUrl { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string WebhookSecret { get; set; } = "";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public sealed class ConnectionInput
{
    public string Name { get; set; } = "Cửa hàng thử nghiệm";
    public string BaseUrl { get; set; } = "";
    public string? ApiKey { get; set; }
    public string? WebhookSecret { get; set; }
    public bool ClearWebhookSecret { get; set; }
}

public sealed class ApiCall
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
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
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string ConnectionId { get; set; } = "";
    public string EventId { get; set; } = "";
    public string Event { get; set; } = "";
    public string PayloadJson { get; set; } = "{}";
    public string SignatureStatus { get; set; } = "valid";
    public string Source { get; set; } = "pos";
    public Dictionary<string, string> Headers { get; set; } = new();
    public string State { get; set; } = "received";
    public int ReceivedCount { get; set; } = 1;
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
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
