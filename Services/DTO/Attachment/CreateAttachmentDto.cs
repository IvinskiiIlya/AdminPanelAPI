using System.ComponentModel.DataAnnotations;

namespace Services.DTO.Attachment;

public class CreateAttachmentDto
{
    [Required]
    public int FeedbackId { get; set; }

    [Required]
    public string FilePath { get; set; }

    [Required]
    public string FileType { get; set; }
}