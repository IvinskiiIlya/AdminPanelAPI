namespace AdminPanelAPI.Services.DTOs.Update;

public class UpdateResponseDto
{
    public int Id { get; set; } // Обязательно для обновления
    public string? ResponseMessage { get; set; }
}