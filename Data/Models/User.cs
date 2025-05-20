using Microsoft.AspNetCore.Identity;

namespace Data.Models;

public class User : IdentityUser<int>
{
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    
    public User(string name, string email)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Email = email;
        UserName = email; 
    }
}