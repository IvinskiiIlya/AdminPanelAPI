namespace Application.DTO.Feedback;

public class DisplayFeedbackDto
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public int CategoryId { get; set; }
    public int StatusId { get; set; }

    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}