namespace Domain.Models;

public class Response
{
    public int Id { get; set; }
    public int FeedbackId { get; set; }
    public int UserId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Feedback Feedback { get; set; } = null!;
    public User User { get; set; } = null!;
    
    public Response(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}