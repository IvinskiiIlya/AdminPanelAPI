using System.ComponentModel.DataAnnotations;

namespace Services.DTO.Category;

public class UpdateCategoryDto
{
    [Required]
    public int Id { get; set; }

    [MaxLength(50)]
    public string? Name { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }
}