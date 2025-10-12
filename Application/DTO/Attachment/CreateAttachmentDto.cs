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
    [BindNever]
    public string FilePath { get; set; }
    [Required]
    public IFormFile File { get; set; }
    [Required]
    public string FileType { get; set; }
}