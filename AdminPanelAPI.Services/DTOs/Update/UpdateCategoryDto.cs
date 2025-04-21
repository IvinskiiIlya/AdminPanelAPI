namespace AdminPanelAPI.Services.DTOs.Update;

public class UpdateCategoryDto
{
    public int Id { get; set; } // Обязательно для обновления
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
}