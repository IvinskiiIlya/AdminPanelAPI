namespace Services.DTO.Feedback;

public class SearchFeedbackDto : PaginationParams
{
    public string? SearchTerm { get; set; }
}