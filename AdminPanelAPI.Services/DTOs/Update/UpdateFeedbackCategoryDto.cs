namespace AdminPanelAPI.Services.DTOs.Update;

public class UpdateFeedbackCategoryDto
{
    public int Id { get; set; } // Обязательно для обновления
    public int? FeedbackId { get; set; }
    public int? CategoryId { get; set; }
}