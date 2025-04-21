namespace AdminPanelAPI.Services.DTOs.Update;

public class UpdateUserDto
{
    public int Id { get; set; } // Обязательно для обновления
    public string? UserName { get; set; }
    public string? Email { get; set; }
}