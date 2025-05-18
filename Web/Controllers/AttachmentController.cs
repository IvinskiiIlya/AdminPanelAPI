using Microsoft.AspNetCore.Authorization;
using Services.Attachments;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.Attachment;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AttachmentController : ControllerBase
{
    
    private readonly IAttachmentService _attachmentService;

    public AttachmentController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    [HttpGet("{feedbackId}")]
    public async Task<ActionResult<List<DisplayAttachmentDto>>> GetAttachmentsByFeedback(int feedbackId)
    {
        var attachments = await _attachmentService.GetAttachmentsByFeedbackAsync(feedbackId);
        return Ok(attachments);
    }

    [HttpPost]
    public async Task<ActionResult> UploadAttachment([FromBody] CreateAttachmentDto dto)
    {
        var attachment = await _attachmentService.CreateAttachmentAsync(dto);
        return CreatedAtAction(nameof(GetAttachmentsByFeedback), new { feedbackId = dto.FeedbackId }, attachment);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAttachment(int id)
    {
        await _attachmentService.DeleteAttachmentAsync(id);
        return NoContent();
    }
}