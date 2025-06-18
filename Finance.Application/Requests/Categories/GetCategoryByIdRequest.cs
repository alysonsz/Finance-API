namespace Finance.Application.Requests.Categories;

public class GetCategoryByIdRequest : Request
{
    public long Id { get; set; }
    public long UserId { get; set; }
}