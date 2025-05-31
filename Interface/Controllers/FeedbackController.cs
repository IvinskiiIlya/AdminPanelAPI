using Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTO.Feedback;
using Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Annotations;

namespace Interface.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    /// <summary>
    /// Получить список всех отзывов
    /// </summary>
    /// <param name="filters">Параметры фильтрации отзывов</param>
    /// <returns>Список отзывов</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получить все отзывы",
        Description = "Возвращает полный список всех отзывов."
    )]
    [SwaggerResponse(200, "Список отзывов успешно получен", typeof(PagedResponse<DisplayFeedbackDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<PagedResponse<DisplayFeedbackDto>>> GetAllFeedbacks([FromQuery] FilterFeedbackDto filters)
    {
        var feedbacks = await _feedbackService.GetAllFeedbacksAsync(filters);
        return Ok(feedbacks);
    }

    /// <summary>
    /// Получить отзыв по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор отзыва</param>
    /// <returns>Отзыв с указанным идентификатором</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Получить отзыв по ID",
        Description = "Возвращает отзыв по заданному идентификатору."
    )]
    [SwaggerResponse(200, "Отзыв успешно найден", typeof(DisplayFeedbackDto))]
    [SwaggerResponse(404, "Отзыв не найден")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayFeedbackDto>> GetFeedbackById(int id)
    {
        var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
        return feedback == null ? NotFound() : Ok(feedback);
    }

    /// <summary>
    /// Создать новый отзыв
    /// </summary>
    /// <param name="dto">Данные для создания отзыва</param>
    /// <returns>Созданный отзыв</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Создать отзыв",
        Description = "Создает новый отзыв с указанными данными."
    )]
    [SwaggerResponse(201, "Отзыв успешно создан", typeof(DisplayFeedbackDto))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayFeedbackDto>> CreateFeedback([FromBody] CreateFeedbackDto dto)
    {
        var feedback = await _feedbackService.CreateFeedbackAsync(dto);
        return CreatedAtAction(nameof(GetFeedbackById), new { id = feedback.Id }, feedback);
    }

    /// <summary>
    /// Обновить существующий отзыв
    /// </summary>
    /// <param name="id">Идентификатор отзыва</param>
    /// <param name="dto">Данные для обновления отзыва</param>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Обновить отзыв",
        Description = "Обновляет данные отзыва по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Отзыв успешно обновлен")]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Отзыв не найден")]
    public async Task<ActionResult> UpdateFeedback(int id, [FromBody] UpdateFeedbackDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Идентификаторы не совпадают.");
        await _feedbackService.UpdateFeedbackAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Удалить отзыв по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор отзыва</param>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Удалить отзыв",
        Description = "Удаляет отзыв по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Отзыв успешно удален")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Отзыв не найден")]
    public async Task<ActionResult> DeleteFeedback(int id)
    {
        await _feedbackService.DeleteFeedbackAsync(id);
        return NoContent();
    }
}
