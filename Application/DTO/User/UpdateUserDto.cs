using System.ComponentModel.DataAnnotations;

namespace Application.DTO.User;

public class UpdateUserDto
{
    [Required]
    public string Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string? UserName { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string? Email { get; set; }
}