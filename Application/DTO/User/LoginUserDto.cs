using System.ComponentModel.DataAnnotations;

namespace Application.DTO.User;

public class LoginUserDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
}