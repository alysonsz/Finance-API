namespace Finance.Application.Requests.Categories;

public class GetAllCategoriesRequest : PagedRequest
{
    public new long UserId { get; set; }
}