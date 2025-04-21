namespace AdminPanelAPI.Data.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    
    public ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}