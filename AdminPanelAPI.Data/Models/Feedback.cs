namespace AdminPanelAPI.Data.Models;

public class Feedback
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FeedbackType { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string Status { get; set; } = "Новый";
    
    public User User { get; set; } = null!;
    public ICollection<FeedbackCategory> FeedbackCategories { get; set; } = new List<FeedbackCategory>();
    public ICollection<Response> Responses { get; set; } = new List<Response>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}