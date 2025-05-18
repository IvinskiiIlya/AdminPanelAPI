using System.ComponentModel.DataAnnotations;

namespace Services.DTO.Status;

public class UpdateStatusDto
{
    [Required]
    public int Id { get; set; }

    [MaxLength(20)]
    public string? Name { get; set; }
}