namespace Services.DTO.User;

public class DisplayUserDto
{
    public int Id { get; set; }
    public int RoleId { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
}