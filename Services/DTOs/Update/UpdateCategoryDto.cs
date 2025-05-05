using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Update;

public class UpdateCategoryDto
{
    [Required]
    public int Id { get; set; } 
    [Required]
    public string? CategoryName { get; set; }
    [Required]
    public string? Description { get; set; }
}