namespace Application.DTO.Status;

public class FilterStatusDto : PaginationParams
{
    public string? Name { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; } = "Id";
    public string? SortOrder { get; set; } = "asc";
}