using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class Response
{
    public int Id { get; set; }
    public int FeedbackId { get; set; }
    public string UserId { get; set; }
    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public Feedback Feedback { get; set; } = null!;
    public IdentityUser User { get; set; } = null!;
    
    public Response() { }
    public Response(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }
}