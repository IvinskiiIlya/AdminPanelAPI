using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Feedback;

public class CreateFeedbackDto
{
    [Required]
    public int UserId { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int StatusId { get; set; }

    [Required]
    [MinLength(10)]
    [MaxLength(1000)]
    public string Message { get; set; }
}