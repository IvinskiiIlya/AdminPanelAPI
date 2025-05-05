using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Update;

public class UpdateFeedbackDto
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string? FeedbackType { get; set; }
    [Required]
    public string? Message { get; set; }
    [Required]
    public string? Status { get; set; }
}