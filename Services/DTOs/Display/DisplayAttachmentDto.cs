namespace Services.DTOs.Display;

public class DisplayAttachmentDto
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public string FileType { get; set; }
    public DateTime CreatedAt { get; set; }
    public int FeedbackId { get; set; }
}