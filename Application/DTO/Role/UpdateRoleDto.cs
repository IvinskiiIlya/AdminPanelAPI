using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Role;

public class UpdateRoleDto
{
    [Required]
    public string Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string? Name { get; set; }
}