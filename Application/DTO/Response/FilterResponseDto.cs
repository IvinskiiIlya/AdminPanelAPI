namespace Application.DTO.Response;

public class FilterResponseDto : PaginationParams
{
    public int? FeedbackId { get; set; }
    public string? UserId { get; set; }
    public string? Message { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; } = "Id";
    public string? SortOrder { get; set; } = "asc";
}