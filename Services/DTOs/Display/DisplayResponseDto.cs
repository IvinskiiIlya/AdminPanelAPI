namespace Services.DTOs.Display;

public class DisplayResponseDto
{
    public int Id { get; set; }
    public string ResponseMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public int FeedbackId { get; set; }
    public int AdminUserId { get; set; }
}