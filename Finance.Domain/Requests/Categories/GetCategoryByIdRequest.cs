namespace Finance.Domain.Requests.Categories;

public class GetCategoryByIdRequest : Request
{
    public long Id { get; set; }
}