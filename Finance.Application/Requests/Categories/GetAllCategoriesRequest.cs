namespace Finance.Application.Requests.Categories;

public class GetAllCategoriesRequest : PagedRequest
{
    public long UserId { get; set; }
}