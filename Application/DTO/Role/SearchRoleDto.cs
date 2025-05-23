namespace Application.DTO.Role;

public class SearchRoleDto : PaginationParams
{
    public string? SearchTerm { get; set; }
}