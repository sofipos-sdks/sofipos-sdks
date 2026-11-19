namespace SofiPOS.FnB.Demo;

// Truy cập được điều phối bởi WorkspaceSession; không lưu database hoặc file.
public sealed class DemoStore
{
    public const int HistoryLimit = 200;
    private readonly List<ApiCall> _calls = new();
    private readonly List<WebhookRecord> _webhooks = new();
    public ConnectionRecord? Connection { get; set; }
    public IReadOnlyList<ApiCall> Calls => _calls;
    public IReadOnlyList<WebhookRecord> Webhooks => _webhooks;

    public void AddCall(ApiCall call)
    {
        _calls.Insert(0, call);
        if (_calls.Count > HistoryLimit) _calls.RemoveAt(_calls.Count - 1);
    }

    public bool AddWebhook(WebhookRecord record)
    {
        var existing = record.SignatureStatus == "valid"
            ? _webhooks.FirstOrDefault(x => x.ConnectionId == record.ConnectionId && x.EventId == record.EventId && x.SignatureStatus == "valid") : null;
        if (existing != null) { existing.ReceivedCount++; return false; }
        _webhooks.Insert(0, record);
        if (_webhooks.Count > HistoryLimit) _webhooks.RemoveAt(_webhooks.Count - 1);
        return true;
    }

    public void ClearHistory() { _calls.Clear(); _webhooks.Clear(); }
}
