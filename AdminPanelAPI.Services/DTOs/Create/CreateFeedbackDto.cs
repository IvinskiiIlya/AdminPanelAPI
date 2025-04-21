namespace AdminPanelAPI.Services.DTOs.Create;

public class CreateFeedbackDto
{
    public string FeedbackType { get; set; }
    public string Message { get; set; }
    public int UserId { get; set; }
}