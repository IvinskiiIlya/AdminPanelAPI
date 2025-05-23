using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Role;

public class UpdateRoleDto
{
    [Required]
    public int Id { get; set; }

    [MaxLength(20)]
    public string? Name { get; set; }
}