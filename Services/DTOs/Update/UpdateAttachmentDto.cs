using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Update;

public class UpdateAttachmentDto
{
    [Required]
    public int Id { get; set; } 
    [Required]
    public string? FilePath { get; set; }
    [Required]
    public string? FileType { get; set; }
}