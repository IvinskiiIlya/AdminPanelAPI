using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTO.Attachment;
using Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Interface.Controllers;

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

    /// <summary>
    /// Получить список вложений по идентификатору отзыва (FeedbackId).
    /// </summary>
    /// <param name="feedbackId">Идентификатор отзыва</param>
    /// <returns>Список вложений</returns>
    [HttpGet("{feedbackId}")]
    [SwaggerOperation(
        Summary = "Получить вложения по отзыву",
        Description = "Возвращает список всех вложений, связанных с указанным отзывом."
    )]
    [SwaggerResponse(200, "Список вложений успешно получен", typeof(List<DisplayAttachmentDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<List<DisplayAttachmentDto>>> GetAttachmentsByFeedback(int feedbackId)
    {
        var attachments = await _attachmentService.GetAttachmentsByFeedbackAsync(feedbackId);
        return Ok(attachments);
    }

    /// <summary>
    /// Загрузить новое вложение.
    /// </summary>
    /// <param name="dto">Данные для создания вложения</param>
    /// <returns>Созданное вложение</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Загрузить вложение",
        Description = "Создает новое вложение и связывает его с отзывом."
    )]
    [SwaggerResponse(201, "Вложение успешно создано", typeof(DisplayAttachmentDto))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult> UploadAttachment([FromBody] CreateAttachmentDto dto)
    {
        var attachment = await _attachmentService.CreateAttachmentAsync(dto);
        return CreatedAtAction(nameof(GetAttachmentsByFeedback), new { feedbackId = dto.FeedbackId }, attachment);
    }

    /// <summary>
    /// Удалить вложение по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор вложения</param>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Удалить вложение",
        Description = "Удаляет вложение по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Вложение успешно удалено")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Вложение не найдено")]
    public async Task<ActionResult> DeleteAttachment(int id)
    {
        await _attachmentService.DeleteAttachmentAsync(id);
        return NoContent();
    }
}
