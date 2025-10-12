using Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTO.Attachment;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Annotations;

namespace Interface.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class AttachmentController : ControllerBase
{
    
    private readonly IWebHostEnvironment _env;
    private readonly IAttachmentService _attachmentService;
    private readonly IFeedbackService _feedbackService;
    private readonly ICategoryService _categoryService;

    public AttachmentController(IWebHostEnvironment env, IAttachmentService attachmentService, IFeedbackService feedbackService, ICategoryService categoryService)
    {
        _env = env;
        _attachmentService = attachmentService;
        _feedbackService = feedbackService;
        _categoryService = categoryService;
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
    /// Получить список вложений пользователя.
    /// </summary>
    /// <returns>Список вложений</returns>
    [HttpGet("attachments")]
    [SwaggerOperation(
        Summary = "Получить вложения текущего пользователя",
        Description = "Возвращает список вложений текущего авторизованного пользователя."
    )]
    [SwaggerResponse(200, "Список вложений успешно получен", typeof(List<DisplayAttachmentDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<List<DisplayAttachmentDto>>> GetAttachmentsByUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var attachments = await _attachmentService.GetAttachmentsByUserIdAsync(userId);
        return Ok(attachments);
    }

    /// <summary>
    /// Загрузить новое вложение.
    /// </summary>
    /// <param name="dto">Данные для создания вложения</param>
    /// <returns>Созданное вложение</returns>
    [HttpPost]
    [Authorize(Roles = "Пользователь")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(
        Summary = "Загрузить вложение",
        Description = "Создает новое вложение и связывает его с отзывом."
    )]
    [SwaggerResponse(201, "Вложение успешно создано", typeof(DisplayAttachmentDto))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayAttachmentDto>> UploadAttachment([FromForm] CreateAttachmentDto dto)
    {
        if (dto.File == null || dto.File.Length == 0)
            return BadRequest("Файл не загружен");

        var uploadFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "assets", "attachments");
        if (!Directory.Exists(uploadFolder))
            Directory.CreateDirectory(uploadFolder);

        var extension = Path.GetExtension(dto.File.FileName);
        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.File.CopyToAsync(stream);
        }

        dto.FilePath = Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
        dto.FileType = dto.File.ContentType;

        var attachment = await _attachmentService.CreateAttachmentAsync(dto);

        return CreatedAtAction(nameof(GetAttachmentById), new { id = attachment.Id }, attachment);
    }

    /// <summary>
    /// Обновить существующее вложение.
    /// </summary>
    /// <param name="id">Идентификатор вложения</param>
    /// <param name="dto">Данные для обновления вложения</param>
    [HttpPut("{id}")]
    [Authorize(Roles = "Пользователь")]
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
