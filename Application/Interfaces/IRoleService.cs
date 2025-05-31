using Application.DTO;
using Application.DTO.Role;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<PagedResponse<DisplayRoleDto>> GetAllRolesAsync(FilterRoleDto filters);
        Task<DisplayRoleDto?> GetRoleByIdAsync(string id);
        Task<DisplayRoleDto> CreateRoleAsync(string name);
        Task UpdateRoleAsync(string id, UpdateRoleDto updateRoleDto);
        Task DeleteRoleAsync(string id);
    }
}