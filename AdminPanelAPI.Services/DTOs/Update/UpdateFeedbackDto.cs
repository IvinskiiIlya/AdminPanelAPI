namespace AdminPanelAPI.Services.DTOs.Update;

public class UpdateFeedbackDto
{
    public int Id { get; set; } // Обязательно для обновления
    public string? FeedbackType { get; set; }
    public string? Message { get; set; }
    public string? Status { get; set; }
}