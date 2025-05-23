using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Category;

public class CreateCategoryDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }
}