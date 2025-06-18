namespace Finance.Application.Requests.Categories;

public class DeleteCategoryRequest : Request
{
    public long Id { get; set; }
    public long UserId { get; set; }
}