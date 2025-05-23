namespace Services.DTO.Status;

public class SearchStatusDto : PaginationParams
{
    public string? SearchTerm { get; set; }
}