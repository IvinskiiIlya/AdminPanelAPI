using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Feedback;

public class UpdateFeedbackDto
{
    [Required]
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? CategoryId { get; set; }

    public int? StatusId { get; set; }

    [MinLength(10)]
    [MaxLength(1000)]
    public string? Message { get; set; }
}