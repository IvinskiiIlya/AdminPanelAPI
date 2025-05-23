using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Status;

public class CreateStatusDto
{
    [MaxLength(20)]
    public string? Name { get; set; }
}