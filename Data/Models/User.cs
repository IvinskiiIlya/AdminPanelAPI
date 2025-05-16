namespace Data.Models;

public class User
{
    public int Id { get; set; }
    
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public string Name { get; set; }
    public string Email { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    
    public User(string name, string email, int roleId)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        RoleId = roleId;
    }
}