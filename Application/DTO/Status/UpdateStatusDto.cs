using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Status;

public class UpdateStatusDto
{
    [Required]
    public int Id { get; set; }

    [MaxLength(20)]
    public string? Name { get; set; }
}