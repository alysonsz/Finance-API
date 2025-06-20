using System.ComponentModel.DataAnnotations;

namespace Finance.Application.Requests.Categories;

public class UpdateCategoryRequest : Request
{
    public long Id { get; set; }
    public new long UserId { get; set; }

    [Required(ErrorMessage = "Título inválido")]
    [MaxLength(80, ErrorMessage = "O título deve conter até 80 caracteres")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Descrição inválida")]
    public string? Description { get; set; }
}