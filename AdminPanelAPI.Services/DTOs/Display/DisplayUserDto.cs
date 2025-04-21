namespace AdminPanelAPI.Services.DTOs.Display;

public class DisplayUserDto
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
}