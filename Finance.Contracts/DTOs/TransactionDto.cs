namespace Finance.Contracts.DTOs;

public class CategoryDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
}
