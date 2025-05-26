using Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.DTO.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace Interface.Controllers;

// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class ResponseController : ControllerBase
{
    
    private readonly IResponseService _responseService;

    public ResponseController(IResponseService responseService)
    {
        _responseService = responseService;
    }
    
    /// <summary>
    /// Получить все ответы с возможностью фильтрации
    /// </summary>
    /// <param name="filters">Параметры фильтрации ответов</param>
    /// <returns>Список ответов</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получить все ответы",
        Description = "Возвращает список всех ответов с применением фильтров."
    )]
    [SwaggerResponse(200, "Список ответов успешно получен", typeof(PagedResponse<DisplayResponseDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<PagedResponse<DisplayResponseDto>>> GetAllResponses([FromQuery] FilterResponseDto filters)
    {
        var responses = await _responseService.GetAllResponsesAsync(filters);
        return Ok(responses);
    }
    
    /// <summary>
    /// Получить ответ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор ответа</param>
    /// <returns>Ответ с указанным идентификатором</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Получить ответ по ID",
        Description = "Возвращает ответ по указанному идентификатору."
    )]
    [SwaggerResponse(200, "Ответ успешно получен", typeof(DisplayResponseDto))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Ответ не найден")]
    public async Task<ActionResult<DisplayResponseDto>> GetResponseById(int id)
    {
        var response = await _responseService.GetResponseByIdAsync(id);
        return response == null ? NotFound() : Ok(response);
    }

    /// <summary>
    /// Получить список ответов по идентификатору отзыва
    /// </summary>
    /// <param name="feedbackId">Идентификатор отзыва</param>
    /// <returns>Список ответов</returns>
    [HttpGet("by-feedback/{feedbackId}")]
    [SwaggerOperation(
        Summary = "Получить ответы по отзыву",
        Description = "Возвращает список всех ответов, связанных с указанным отзывом."
    )]
    [SwaggerResponse(200, "Список ответов успешно получен", typeof(List<DisplayResponseDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<List<DisplayResponseDto>>> GetResponsesByFeedback(int feedbackId)
    {
        var responses = await _responseService.GetResponsesByFeedbackAsync(feedbackId);
        return Ok(responses);
    }

    /// <summary>
    /// Создать новый ответ
    /// </summary>
    /// <param name="dto">Данные для создания ответа</param>
    /// <returns>Созданный ответ</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Создать ответ",
        Description = "Создает новый ответ и связывает его с отзывом."
    )]
    [SwaggerResponse(201, "Ответ успешно создан", typeof(DisplayResponseDto))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayResponseDto>> CreateResponse([FromBody] CreateResponseDto dto)
    {
        var response = await _responseService.CreateResponseAsync(dto);
        return CreatedAtAction(
            nameof(GetResponsesByFeedback),
            new { feedbackId = dto.FeedbackId },
            response
        );
    }

    /// <summary>
    /// Обновить существующий ответ
    /// </summary>
    /// <param name="id">Идентификатор ответа</param>
    /// <param name="dto">Данные для обновления ответа</param>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Обновить ответ",
        Description = "Обновляет данные ответа по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Ответ успешно обновлен")]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Ответ не найден")]
    public async Task<ActionResult> UpdateResponse(int id, [FromBody] UpdateResponseDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Идентификаторы не совпадают.");
        await _responseService.UpdateResponseAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Удалить ответ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор ответа</param>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Удалить ответ",
        Description = "Удаляет ответ по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Ответ успешно удален")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Ответ не найден")]
    public async Task<ActionResult> DeleteResponse(int id)
    {
        await _responseService.DeleteResponseAsync(id);
        return NoContent();
    }
}
