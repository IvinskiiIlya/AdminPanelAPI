namespace Application.DTO.Role;

public class FilterRoleDto : PaginationParams
{
    public string? Name { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; } = "Name";
    public string? SortOrder { get; set; } = "asc";
}