using System.ComponentModel.DataAnnotations;

namespace Application.DTO.User;

public class UpdateUserDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public int? RoleId { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Name { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(50)]
    public string? Email { get; set; }

    public DateTime? LastLogin { get; set; }
}