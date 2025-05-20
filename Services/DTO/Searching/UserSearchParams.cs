using Services.DTO.Pagination;

namespace Services.DTO.Searching;

public class UserSearchParams : PaginationParams
{
    public string? SearchTerm { get; set; }
}