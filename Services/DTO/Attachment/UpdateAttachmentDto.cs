using System.ComponentModel.DataAnnotations;

namespace Services.DTO.Attachment;

public class UpdateAttachmentDto
{
    [Required]
    public int Id { get; set; } 
    
    [Required]
    public int FeedbackId { get; set; }

    [Required]
    public string FilePath { get; set; }

    [Required]
    public string FileType { get; set; }
}