using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Feedback;

public class UpdateFeedbackDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    public string? UserId { get; set; }

    [Required]
    public int? CategoryId { get; set; }

    [Required]
    public int? StatusId { get; set; }

    [Required]
    [MinLength(10)]
    [MaxLength(1000)]
    public string? Message { get; set; }
}