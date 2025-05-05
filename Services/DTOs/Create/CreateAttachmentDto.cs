using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Create;

public class CreateAttachmentDto
{
    [Required]
    public string FilePath { get; set; }
    [Required]
    public string FileType { get; set; }
    [Required]
    public int FeedbackId { get; set; }
}