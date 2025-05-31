using System.ComponentModel.DataAnnotations;

namespace Application.DTO.User;

public class LoginUserDto
{
    [Required(ErrorMessage = "Требуется email")]
    [EmailAddress]
    public string Email { get; set; }

    [Required(ErrorMessage = "Требуется пароль")]
    public string Password { get; set; }
}