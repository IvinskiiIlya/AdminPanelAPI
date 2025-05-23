namespace Application.DTO.Response;

public class SearchResponseDto : PaginationParams
{
    public string? SearchTerm { get; set; }
}