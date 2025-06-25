namespace Finance.Contracts.Requests.Categories;

public class GetCategoryByIdRequest : Request
{
    public long Id { get; set; }
    public new long UserId { get; set; }
}