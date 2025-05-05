using System.ComponentModel.DataAnnotations;

namespace Services.DTOs.Create;

public class CreateResponseDto
{
    [Required]
    public string ResponseMessage { get; set; }
    [Required]
    public int FeedbackId { get; set; }
    [Required]
    public int AdminUserId { get; set; }
}