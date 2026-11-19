using System.Runtime.CompilerServices;
using SofiPOS.FnB.Http;
using SofiPOS.FnB.Models;

namespace SofiPOS.FnB.Services;

/// <summary>Nền chung của các service: giữ <see cref="ApiClient"/> và các tiện ích dựng đường dẫn/phân trang.</summary>
public abstract class ServiceBase
{
    private protected ServiceBase(ApiClient api)
    {
        Api = api;
    }

    private protected ApiClient Api { get; }

    private protected static string Id(Guid id) => id.ToString("D");

    private protected static string Segment(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Giá trị không được rỗng.", nameof(value));
        return Uri.EscapeDataString(value.Trim());
    }

    /// <summary>Query <c>branchId</c> bắt buộc của API FnB; <see cref="Guid.Empty"/> báo lỗi trước khi gửi.</summary>
    private protected static QueryString Branch(Guid branchId) => new QueryString().Add("branchId", RequiredBranch(branchId));

    private protected static Guid RequiredBranch(Guid branchId)
        => branchId == Guid.Empty ? throw new ArgumentException("Vui lòng truyền chi nhánh (branchId).", nameof(branchId)) : branchId;

    private protected static QueryString Page(PageQuery? query)
    {
        var q = new QueryString();
        if (query != null)
        {
            q.Add("pageNumber", query.PageNumber).Add("pageSize", query.PageSize);
        }

        return q;
    }

    /// <summary>Duyệt lần lượt mọi trang, bắt đầu từ trang của <paramref name="query"/>.</summary>
    private protected static async IAsyncEnumerable<T> EnumerateAsync<TQuery, T>(
        TQuery query,
        Func<TQuery, CancellationToken, Task<PagedResult<T>>> loadPage,
        [EnumeratorCancellation] CancellationToken cancellationToken)
        where TQuery : PageQuery
    {
        while (true)
        {
            var page = await loadPage(query, cancellationToken).ConfigureAwait(false);
            foreach (var item in page.Items) yield return item;

            if (!page.MetaData.HasNext || page.Items.Count == 0) yield break;
            query.PageNumber = page.MetaData.CurrentPage + 1;
        }
    }
}
