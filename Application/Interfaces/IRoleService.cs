using Application.DTO;
using Application.DTO.Role;

namespace Application.Interfaces
{
    public interface IRoleService
    {
        Task<PagedResponse<DisplayRoleDto>> GetAllRolesAsync(FilterRoleDto filters);
        Task<DisplayRoleDto?> GetRoleByIdAsync(int id);
        Task<DisplayRoleDto> CreateRoleAsync(string name);
        Task UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto);
        Task DeleteRoleAsync(int id);
    }
}