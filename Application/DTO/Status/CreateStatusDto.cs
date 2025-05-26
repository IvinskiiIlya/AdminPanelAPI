using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Status;

public class CreateStatusDto
{
    [Required]
    public int Id { get; set; }
    
    [MaxLength(20)]
    public string? Name { get; set; }
}