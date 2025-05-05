using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Create;

public class CreateUserDto
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}