using System.Text.Json.Serialization;

namespace Finance.Domain.Responses;

public class PagedResponse<TData> : Response<TData>
{
    [JsonConstructor]
    public PagedResponse(
        TData? data,
        int totalCount,
        int currentPage = 1,
        int pageSize = 25)
        : base(data, 200, null)
    {
        TotalCount = totalCount;
        CurrentPage = currentPage;
        PageSize = pageSize;
    }

    public PagedResponse(string? message, int code = 400)
        : base(default, code, message)
    {
    }

    public int CurrentPage { get; set; }
    public int PageSize { get; set; } = 25;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}