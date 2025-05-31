namespace Application.DTO.User;

public class FilterUserDto : PaginationParams
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? SearchTerm { get; set; } 
}