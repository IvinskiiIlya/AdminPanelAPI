using Microsoft.AspNetCore.Authorization;
using Services.Feedbacks;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.Feedback;
using Swashbuckle.AspNetCore.Annotations;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    /// <returns>Список отзывов</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получить все отзывы",
        Description = "Возвращает полный список всех отзывов."
    )]
    [SwaggerResponse(200, "Список отзывов успешно получен", typeof(List<DisplayFeedbackDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<List<DisplayFeedbackDto>>> GetAllFeedbacks()
    {
        var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
        return Ok(feedbacks.ToList());
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
