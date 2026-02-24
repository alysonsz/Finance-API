namespace Finance.Contracts.Responses.Categories;

public sealed class CategoryOutboxResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long UserId { get; set; }
}
