namespace AdminPanelAPI.Services.DTOs.Create;

public class CreateAdminDto
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } // Hash password before saving
}