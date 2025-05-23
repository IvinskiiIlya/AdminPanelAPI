using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Roles;
using Services.DTO.Role;
using Swashbuckle.AspNetCore.Annotations;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
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
    /// <returns>Список ролей</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получить все роли",
        Description = "Возвращает полный список всех ролей."
    )]
    [SwaggerResponse(200, "Список ролей успешно получен", typeof(List<DisplayRoleDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<List<DisplayRoleDto>>> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles.ToList());
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
    public async Task<ActionResult<DisplayRoleDto>> GetRoleById(int id)
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
    [SwaggerOperation(
        Summary = "Обновить роль",
        Description = "Обновляет имя роли по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Роль успешно обновлена")]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Роль не найдена")]
    public async Task<ActionResult> UpdateRole(int id, [FromBody] string name)
    {
        await _roleService.UpdateRoleAsync(id, name);
        return NoContent();
    }

    /// <summary>
    /// Удалить роль по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор роли</param>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Удалить роль",
        Description = "Удаляет роль по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Роль успешно удалена")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Роль не найдена")]
    public async Task<ActionResult> DeleteRole(int id)
    {
        await _roleService.DeleteRoleAsync(id);
        return NoContent();
    }
}
