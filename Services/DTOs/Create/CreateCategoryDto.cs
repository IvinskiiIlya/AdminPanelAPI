using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Create;

public class CreateCategoryDto
{
    [Required]
    public string CategoryName { get; set; }
    [Required]
    public string? Description { get; set; }
}