namespace Application.DTO.Role;

public class FilterRoleDto : PaginationParams
{
    public string? Name { get; set; }
    public string? SearchTerm { get; set; }
}