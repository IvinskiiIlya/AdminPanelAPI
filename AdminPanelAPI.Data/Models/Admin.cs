namespace AdminPanelAPI.Data.Models;

public class Admin
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public ICollection<Response> Responses { get; set; } = new List<Response>();
}