using Microsoft.AspNetCore.Authorization;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Application.DTO.User;
using Swashbuckle.AspNetCore.Annotations;

namespace Interface.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Получить список пользователей с фильтрацией
    /// </summary>
    /// <param name="filters">Параметры фильтрации пользователей</param>
    /// <returns>Отфильтрованный список пользователей</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получить пользователей с фильтрацией",
        Description = "Возвращает список пользователей с возможностью фильтрации и пагинации"
    )]
    [SwaggerResponse(200, "Список пользователей успешно получен", typeof(List<DisplayUserDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<IActionResult> GetAllUsers([FromQuery] FilterUserDto filters)
    {
        var users = await _userService.GetAllUsersAsync(filters);
        return Ok(users);
    }

    /// <summary>
    /// Получить пользователя по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <returns>Данные пользователя</returns>
    [HttpGet("{id}")]
    [SwaggerOperation(
        Summary = "Получить пользователя по ID",
        Description = "Возвращает детальную информацию о пользователе"
    )]
    [SwaggerResponse(200, "Пользователь успешно найден", typeof(DisplayUserDto))]
    [SwaggerResponse(404, "Пользователь не найден")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayUserDto>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    /// <summary>
    /// Создать нового пользователя
    /// </summary>
    /// <param name="dto">Данные для создания пользователя</param>
    /// <returns>Созданный пользователь</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Создать пользователя",
        Description = "Регистрирует нового пользователя в системе"
    )]
    [SwaggerResponse(201, "Пользователь успешно создан", typeof(DisplayUserDto))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayUserDto>> CreateUser([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Обновить данные пользователя
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    /// <param name="dto">Данные для обновления</param>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Обновить пользователя",
        Description = "Обновляет информацию о существующем пользователе"
    )]
    [SwaggerResponse(204, "Пользователь успешно обновлен")]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Пользователь не найден")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        await _userService.UpdateUserAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Удалить пользователя
    /// </summary>
    /// <param name="id">Идентификатор пользователя</param>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Удалить пользователя",
        Description = "Удаляет пользователя из системы"
    )]
    [SwaggerResponse(204, "Пользователь успешно удален")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Пользователь не найден")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}
