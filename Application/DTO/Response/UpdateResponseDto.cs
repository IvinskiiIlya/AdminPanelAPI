using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Response;

public class UpdateResponseDto
{
    [Required]
    public int Id { get; set; }

    public int? FeedbackId { get; set; }

    public int? UserId { get; set; }

    public string? Message { get; set; }
}