using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Update;

public class UpdateResponseDto
{
    [Required]
    public int Id { get; set; } 
    [Required]
    public string? ResponseMessage { get; set; }
}