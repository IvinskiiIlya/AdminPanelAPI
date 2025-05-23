namespace Services.DTO.Response;

public class SearchResponseDto : PaginationParams
{
    public string? SearchTerm { get; set; }
}