using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Update;

public class UpdateUserDto
{
    [Required]
    public int Id { get; set; } 
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? Email { get; set; }
}