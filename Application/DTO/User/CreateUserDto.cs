using System.ComponentModel.DataAnnotations;

namespace Application.DTO.User;

public class CreateUserDto
{
    [Required]
    [MaxLength(50)]
    public string UserName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }
}