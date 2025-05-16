using Services;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;
using Services.Users;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DisplayUserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users.ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DisplayUserDto>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<DisplayUserDto>> CreateUser([FromBody] CreateUserDto dto)
    {
        var user = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
    {
        await _userService.UpdateUserAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}