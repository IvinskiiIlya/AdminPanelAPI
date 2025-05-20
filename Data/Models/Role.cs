using Microsoft.AspNetCore.Identity;

namespace Data.Models;

public class Role : IdentityRole<int>
{
    public Role() : base() { }
    public Role(string name) : base(name) { }
}