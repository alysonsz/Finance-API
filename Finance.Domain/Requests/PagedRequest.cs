namespace Finance.Domain.Requests;

public abstract class PagedRequest : Request
{
    public int PageSize { get; set; } = 25;
    public int PageNumber { get; set; } = 1;
}