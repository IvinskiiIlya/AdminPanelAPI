using System.ComponentModel.DataAnnotations;

namespace Services.DTO.User;

public class UpdateUserDto
{
    [Required]
    public int Id { get; set; }

    public int? RoleId { get; set; }

    [MaxLength(50)]
    public string? Name { get; set; }

    [EmailAddress]
    [MaxLength(50)]
    public string? Email { get; set; }

    public DateTime? LastLogin { get; set; }
}