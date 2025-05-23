using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Statuses;
using Services.DTO.Status;
using Swashbuckle.AspNetCore.Annotations;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatusController : ControllerBase
{
    
    private readonly IStatusService _statusService;

    public StatusController(IStatusService statusService)
    {
        _statusService = statusService;
    }

    /// <summary>
    /// Получить список всех статусов
    /// </summary>
    /// <returns>Список статусов</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получить все статусы",
        Description = "Возвращает полный список всех статусов."
    )]
    [SwaggerResponse(200, "Список статусов успешно получен", typeof(List<DisplayStatusDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<List<DisplayStatusDto>>> GetAllStatuses()
    {
        var statuses = await _statusService.GetAllStatusesAsync();
        return Ok(statuses.ToList());
    }

    /// <summary>
    /// Получить статус по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор статуса</param>
    /// <returns>Статус с указанным идентификатором</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Получить статус по ID",
        Description = "Возвращает статус по заданному идентификатору."
    )]
    [SwaggerResponse(200, "Статус успешно найден", typeof(DisplayStatusDto))]
    [SwaggerResponse(404, "Статус не найден")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayStatusDto>> GetStatusById(int id)
    {
        var status = await _statusService.GetStatusByIdAsync(id);
        return status == null ? NotFound() : Ok(status);
    }

    /// <summary>
    /// Создать новый статус
    /// </summary>
    /// <param name="name">Имя статуса</param>
    /// <returns>Созданный статус</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Создать статус",
        Description = "Создает новый статус с указанным именем."
    )]
    [SwaggerResponse(201, "Статус успешно создан", typeof(DisplayStatusDto))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayStatusDto>> CreateStatus([FromBody] string name)
    {
        var status = await _statusService.CreateStatusAsync(name);
        return CreatedAtAction(nameof(GetStatusById), new { id = status.Id }, status);
    }

    /// <summary>
    /// Обновить существующий статус
    /// </summary>
    /// <param name="id">Идентификатор статуса</param>
    /// <param name="name">Новое имя статуса</param>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Обновить статус",
        Description = "Обновляет имя статуса по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Статус успешно обновлен")]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Статус не найден")]
    public async Task<ActionResult> UpdateStatus(int id, [FromBody] string name)
    {
        await _statusService.UpdateStatusAsync(id, name);
        return NoContent();
    }

    /// <summary>
    /// Удалить статус по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор статуса</param>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Удалить статус",
        Description = "Удаляет статус по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Статус успешно удален")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Статус не найден")]
    public async Task<ActionResult> DeleteStatus(int id)
    {
        await _statusService.DeleteStatusAsync(id);
        return NoContent();
    }
}
