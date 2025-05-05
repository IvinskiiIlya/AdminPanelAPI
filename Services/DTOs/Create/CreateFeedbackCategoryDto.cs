using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Create;

public class CreateFeedbackCategoryDto
{
    [Required]
    public int FeedbackId { get; set; }
    [Required]
    public int CategoryId { get; set; }
}