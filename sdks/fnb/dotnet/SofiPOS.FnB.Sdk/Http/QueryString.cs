using System.Globalization;
using System.Text;

namespace SofiPOS.FnB.Http;

/// <summary>Dựng query string; bỏ qua tham số <c>null</c> hoặc chuỗi rỗng.</summary>
internal sealed class QueryString
{
    private readonly List<KeyValuePair<string, string>> _items = new();

    public QueryString Add(string name, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value)) _items.Add(new(name, value));
        return this;
    }

    public QueryString Add(string name, Guid? value)
        => value.HasValue ? Add(name, value.Value.ToString("D")) : this;

    public QueryString Add(string name, bool? value)
        => value.HasValue ? Add(name, value.Value ? "true" : "false") : this;

    public QueryString Add(string name, int? value)
        => value.HasValue ? Add(name, value.Value.ToString(CultureInfo.InvariantCulture)) : this;

    public QueryString Add(string name, DateTime? value)
        => value.HasValue ? Add(name, value.Value.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture)) : this;

    public override string ToString()
    {
        if (_items.Count == 0) return string.Empty;
        var sb = new StringBuilder();
        foreach (var (key, value) in _items)
        {
            sb.Append(sb.Length == 0 ? '?' : '&')
              .Append(Uri.EscapeDataString(key))
              .Append('=')
              .Append(Uri.EscapeDataString(value));
        }

        return sb.ToString();
    }
}
