using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Application.DTO.Attachment;

public class CreateAttachmentDto
{
    [Required]
    public int FeedbackId { get; set; }
    [Required]
    public string UserId { get; set; }
    [Required]
    public string FilePath { get; set; }
    public IFormFile File { get; set; }
    [Required]
    public string FileType { get; set; }
}