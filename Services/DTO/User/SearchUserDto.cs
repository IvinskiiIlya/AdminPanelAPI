namespace Services.DTO.User;

public class SearchUserDto : PaginationParams
{
    public string? SearchTerm { get; set; }
}