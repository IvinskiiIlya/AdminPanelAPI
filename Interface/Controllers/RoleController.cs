using Application.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.DTO.Role;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Swashbuckle.AspNetCore.Annotations;

namespace Interface.Controllers;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    /// <summary>
    /// Получить список всех ролей
    /// </summary>
    /// <param name="filters">Параметры фильтрации ролей</param>
    /// <returns>Список ролей</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получить все роли",
        Description = "Возвращает полный список всех ролей."
    )]
    [SwaggerResponse(200, "Список ролей успешно получен", typeof(PagedResponse<DisplayRoleDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<PagedResponse<DisplayRoleDto>>> GetAllRoles([FromQuery] FilterRoleDto filters)
    {
        var roles = await _roleService.GetAllRolesAsync(filters);
        return Ok(roles);
    }

    /// <summary>
    /// Получить роль по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор роли</param>
    /// <returns>Роль с указанным идентификатором</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Получить роль по ID",
        Description = "Возвращает роль по заданному идентификатору."
    )]
    [SwaggerResponse(200, "Роль успешно найдена", typeof(DisplayRoleDto))]
    [SwaggerResponse(404, "Роль не найдена")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayRoleDto>> GetRoleById(string id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        return role == null ? NotFound() : Ok(role);
    }

    /// <summary>
    /// Создать новую роль
    /// </summary>
    /// <param name="name">Имя роли</param>
    /// <returns>Созданная роль</returns>
    [HttpPost]
    [Authorize(Roles = "Администратор")]
    [SwaggerOperation(
        Summary = "Создать роль",
        Description = "Создает новую роль с указанным именем."
    )]
    [SwaggerResponse(201, "Роль успешно создана", typeof(DisplayRoleDto))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayRoleDto>> CreateRole([FromBody] string name)
    {
        var role = await _roleService.CreateRoleAsync(name);
        return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
    }

    /// <summary>
    /// Обновить существующую роль
    /// </summary>
    /// <param name="id">Идентификатор роли</param>
    /// <param name="name">Новое имя роли</param>
    [HttpPut("{id}")]
    [Authorize(Roles = "Администратор")]
    [SwaggerOperation(
        Summary = "Обновить роль",
        Description = "Обновляет имя роли по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Роль успешно обновлена")]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Роль не найдена")]
    public async Task<ActionResult> UpdateRole(string id, [FromBody] UpdateRoleDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Идентификаторы не совпадают.");
        await _roleService.UpdateRoleAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Удалить роль по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор роли</param>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Администратор")]
    [SwaggerOperation(
        Summary = "Удалить роль",
        Description = "Удаляет роль по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Роль успешно удалена")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Роль не найдена")]
    public async Task<ActionResult> DeleteRole(string id)
    {
        await _roleService.DeleteRoleAsync(id);
        return NoContent();
    }
}
