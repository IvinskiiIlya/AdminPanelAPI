namespace Application.DTO.Status;

public class FilterStatusDto : PaginationParams
{
    public string? Name { get; set; }
    public string? SearchTerm { get; set; }
}