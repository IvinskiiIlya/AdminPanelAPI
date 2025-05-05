using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Update;

public class UpdateFeedbackCategoryDto
{
    [Required]
    public int Id { get; set; } 
    [Required]
    public int? FeedbackId { get; set; }
    [Required]
    public int? CategoryId { get; set; }
}