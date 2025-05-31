namespace Application.DTO.Category;

public class FilterCategoryDto : PaginationParams
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; } = "Id";
    public string? SortOrder { get; set; } = "asc";
}