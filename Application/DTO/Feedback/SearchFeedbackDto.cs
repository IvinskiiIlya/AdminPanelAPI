namespace Application.DTO.Feedback;

public class SearchFeedbackDto : PaginationParams
{
    public string? SearchTerm { get; set; }
}