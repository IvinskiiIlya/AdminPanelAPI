namespace Services.DTO.Response;

public class DisplayResponseDto
{
    public int Id { get; set; }
    public int FeedbackId { get; set; }
    public int UserId { get; set; }

    public string Message { get; set; }
    public DateTime CreatedAt { get; set; }
}