using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Create;

public class CreateFeedbackDto
{
    [Required]
    public string FeedbackType { get; set; }
    [Required]
    [MinLength(10)]
    [MaxLength(1000)]
    public string Message { get; set; }
    [Required]
    public int UserId { get; set; }
}