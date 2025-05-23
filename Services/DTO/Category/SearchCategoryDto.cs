namespace Services.DTO.Category;

public class SearchCategoryDto : PaginationParams
{
    public string? SearchTerm { get; set; }
}