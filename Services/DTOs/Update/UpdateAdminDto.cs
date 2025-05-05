using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Update;

public class UpdateAdminDto
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string? UserName { get; set; }
    [Required]
    public string? Email { get; set; }
}