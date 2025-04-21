namespace AdminPanelAPI.Data.Models;

public class Response
{
    public int Id { get; set; }
    public int FeedbackId { get; set; }
    public int AdminUserId { get; set; }
    public string ResponseMessage { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Feedback Feedback { get; set; } = null!;
    public Admin AdminUser { get; set; } = null!;
}