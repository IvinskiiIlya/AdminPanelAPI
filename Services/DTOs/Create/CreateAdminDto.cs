using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Create;

public class CreateAdminDto
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; } // Hash password before saving
}