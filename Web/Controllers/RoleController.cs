using Microsoft.AspNetCore.Mvc;
using Services.Roles;
using Services.DTO.Role;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DisplayRoleDto>>> GetAllRoles()
    {
        var roles = await _roleService.GetAllRolesAsync();
        return Ok(roles.ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DisplayRoleDto>> GetRoleById(int id)
    {
        var role = await _roleService.GetRoleByIdAsync(id);
        return role == null ? NotFound() : Ok(role);
    }

    [HttpPost]
    public async Task<ActionResult<DisplayRoleDto>> CreateRole([FromBody] string name)
    {
        var role = await _roleService.CreateRoleAsync(name);
        return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, role);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateRole(int id, [FromBody] string name)
    {
        await _roleService.UpdateRoleAsync(id, name);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteRole(int id)
    {
        await _roleService.DeleteRoleAsync(id);
        return NoContent();
    }
}