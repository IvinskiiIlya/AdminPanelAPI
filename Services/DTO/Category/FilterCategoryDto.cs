namespace Services.DTO.Category;

public class FilterCategoryDto : PaginationParams
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}