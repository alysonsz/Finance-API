namespace Finance.Domain.Models;

public class Category
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public long UserId { get; set; }
    public User? User { get; set; }
}