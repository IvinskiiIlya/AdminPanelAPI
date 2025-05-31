using Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO.Status;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Annotations;

namespace Interface.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
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
    /// <param name="filters">Параметры фильтрации статусов</param>
    /// <returns>Список статусов</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получить все статусы",
        Description = "Возвращает полный список всех статусов."
    )]
    [SwaggerResponse(200, "Список статусов успешно получен", typeof(PagedResponse<DisplayStatusDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<PagedResponse<DisplayStatusDto>>> GetAllStatuses([FromQuery] FilterStatusDto filters)
    {
        var statuses = await _statusService.GetAllStatusesAsync(filters);
        return Ok(statuses);
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
    [Authorize(Roles = "Администратор")]
    [SwaggerOperation(
        Summary = "Создать статус",
        Description = "Создает новый статус с указанным именем."
    )]
    [SwaggerResponse(201, "Статус успешно создан", typeof(DisplayStatusDto))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayStatusDto>> CreateStatus([FromBody] CreateStatusDto dto)
    {
        var status = await _statusService.CreateStatusAsync(dto);
        return CreatedAtAction(nameof(GetAllStatuses), status);
    }

    /// <summary>
    /// Обновить существующий статус
    /// </summary>
    /// <param name="id">Идентификатор статуса</param>
    /// <param name="name">Новое имя статуса</param>
    [HttpPut("{id}")]
    [Authorize(Roles = "Администратор")]
    [SwaggerOperation(
        Summary = "Обновить статус",
        Description = "Обновляет имя статуса по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Статус успешно обновлен")]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Статус не найден")]
    public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Идентификаторы не совпадают.");
        
        await _statusService.UpdateStatusAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Удалить статус по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор статуса</param>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Администратор")]
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
