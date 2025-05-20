using System.ComponentModel.DataAnnotations;

namespace Services.DTO.User;

public class CreateUserDto
{
    [Required]
    public int RoleId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string Email { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; }
}