namespace Domain.Models;

public class Feedback
{
    public int Id { get; set; }
    
    public string UserId { get; set; }
    public int CategoryId { get; set; }
    public int StatusId { get; set; }

    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public Status Status { get; set; } = null!;

    public string Message { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Feedback() { }
    public Feedback(string message)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
    }

    public ICollection<Response> Responses { get; set; } = new List<Response>();
    public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}