namespace AdminPanelAPI.Data.Models;

public class FeedbackCategory
{
    public int Id { get; set; }
    public int FeedbackId { get; set; }
    public int CategoryId { get; set; }
    
    public Feedback Feedback { get; set; } = null!;
    public Category Category { get; set; } = null!;
}