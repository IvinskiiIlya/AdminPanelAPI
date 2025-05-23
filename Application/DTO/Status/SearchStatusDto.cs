namespace Application.DTO.Status;

public class SearchStatusDto : PaginationParams
{
    public string? SearchTerm { get; set; }
}