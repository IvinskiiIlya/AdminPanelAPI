using System.ComponentModel.DataAnnotations;

namespace Application.DTO.Response;

public class CreateResponseDto
{
    [Required]
    public int FeedbackId { get; set; }

    [Required]
    public int UserId { get; set; } 

    [Required]
    public string Message { get; set; }  
}