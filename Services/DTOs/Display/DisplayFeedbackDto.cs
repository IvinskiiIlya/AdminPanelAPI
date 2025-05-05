namespace Services.DTOs.Display;

public class DisplayFeedbackDto
{
    public int Id { get; set; }
    public string FeedbackType { get; set; }
    public string Message { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UserId { get; set; }
    
    public List<DisplayAttachmentDto> Attachments { get; set; }
    public List<DisplayCategoryDto> Categories { get; set; }
}