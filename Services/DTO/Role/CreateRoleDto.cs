using System.ComponentModel.DataAnnotations;

namespace Services.DTO.Role;

public class CreateRoleDto
{
    [Required]
    [MaxLength(20)]
    public string Name { get; set; }
}