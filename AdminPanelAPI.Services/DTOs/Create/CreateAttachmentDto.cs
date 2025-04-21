namespace AdminPanelAPI.Services.DTOs.Create;

public class CreateAttachmentDto
{
    public string FilePath { get; set; }
    public string FileType { get; set; }
    public int FeedbackId { get; set; }
}