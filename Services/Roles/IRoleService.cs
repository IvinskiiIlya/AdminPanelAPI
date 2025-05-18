using Services.DTO.Role;

namespace Services.Roles;

public interface IRoleService
{
    Task<IEnumerable<DisplayRoleDto>> GetAllRolesAsync();
    Task<DisplayRoleDto?> GetRoleByIdAsync(int id);
    Task<DisplayRoleDto> CreateRoleAsync(string name);
    Task UpdateRoleAsync(int id, string name);
    Task DeleteRoleAsync(int id);
}