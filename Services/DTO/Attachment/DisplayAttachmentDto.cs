namespace Services.DTO.Attachment;

public class DisplayAttachmentDto
{
    public int Id { get; set; }
    public int FeedbackId { get; set; }

    public string FilePath { get; set; }
    public string FileType { get; set; }

    public DateTime CreatedAt { get; set; }
}