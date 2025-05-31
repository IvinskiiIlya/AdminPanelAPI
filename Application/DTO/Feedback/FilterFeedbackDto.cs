namespace Application.DTO.Feedback;

public class FilterFeedbackDto : PaginationParams
{
    public string? UserId { get; set; }
    public int? CategoryId { get; set; }
    public int? StatusId { get; set; }
    public string? Message { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SearchTerm { get; set; }
    public string? SortColumn { get; set; } = "Id";
    public string? SortOrder { get; set; } = "asc";
}