namespace AdminPanelAPI.Services.DTOs.Update;

public class UpdateAttachmentDto
{
    public int Id { get; set; } // Обязательно для обновления
    public string? FilePath { get; set; }
    public string? FileType { get; set; }
}