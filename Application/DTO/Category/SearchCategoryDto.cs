namespace Application.DTO.Category;

public class SearchCategoryDto : PaginationParams
{
    public string? SearchTerm { get; set; }
}