namespace Application.DTO.Response;

public class FilterResponseDto : PaginationParams
{
    public int? FeedbackId { get; set; }
    public int? UserId { get; set; }
    public string? Message { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
}