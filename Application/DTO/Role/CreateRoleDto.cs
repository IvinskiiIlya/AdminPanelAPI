using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Role;

public class CreateRoleDto
{
    [Required]
    [MaxLength(20)]
    public string Name { get; set; }
}