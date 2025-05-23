using Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTO.Attachment;
using Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Interface.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttachmentController : ControllerBase
{
    
    private readonly IAttachmentService _attachmentService;

    public AttachmentController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    /// <summary>
    /// Получить список всех вложений с фильтрацией и пагинацией.
    /// </summary>
    /// <param name="filters">Параметры фильтрации для вложений</param>
    /// <returns>Постраничный список вложений</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получить все вложения",
        Description = "Возвращает постраничный список всех вложений с учетом фильтров."
    )]
    [SwaggerResponse(200, "Список вложений успешно получен", typeof(PagedResponse<DisplayAttachmentDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<PagedResponse<DisplayAttachmentDto>>> GetAllAttachments([FromQuery] FilterAttachmentDto filters)
    {
        var attachments = await _attachmentService.GetAllAttachmentsAsync(filters);
        return Ok(attachments);
    }

    /// <summary>
    /// Получить вложение по идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор вложения</param>
    /// <returns>Вложение с указанным идентификатором</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Получить вложение по ID",
        Description = "Возвращает вложение по заданному идентификатору."
    )]
    [SwaggerResponse(200, "Вложение успешно найдено", typeof(DisplayAttachmentDto))]
    [SwaggerResponse(404, "Вложение не найдено")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayAttachmentDto>> GetAttachmentById(int id)
    {
        var attachment = await _attachmentService.GetAttachmentByIdAsync(id);
        return attachment == null ? NotFound() : Ok(attachment);
    }

    /// <summary>
    /// Получить список вложений по идентификатору отзыва (FeedbackId).
    /// </summary>
    /// <param name="feedbackId">Идентификатор отзыва</param>
    /// <returns>Список вложений</returns>
    [HttpGet("feedback/{feedbackId}")]
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
    public async Task<ActionResult<DisplayAttachmentDto>> UploadAttachment([FromBody] CreateAttachmentDto dto)
    {
        var attachment = await _attachmentService.CreateAttachmentAsync(dto);
        return CreatedAtAction(nameof(GetAttachmentById), new { id = attachment.Id }, attachment);
    }

    /// <summary>
    /// Обновить существующее вложение.
    /// </summary>
    /// <param name="id">Идентификатор вложения</param>
    /// <param name="dto">Данные для обновления вложения</param>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Обновить вложение",
        Description = "Обновляет данные вложения по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Вложение успешно обновлено")]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Вложение не найдено")]
    public async Task<ActionResult> UpdateAttachment(int id, [FromBody] UpdateAttachmentDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Идентификаторы не совпадают.");

        await _attachmentService.UpdateAttachmentAsync(dto);
        return NoContent();
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
