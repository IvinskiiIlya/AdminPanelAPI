using Repositories.Roles;
using Data.Models;
using Services.DTO.Role;

namespace Services.Roles
{
    public class RoleService : IRoleService
    {
        
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<DisplayRoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(r => new DisplayRoleDto
            {
                Id = r.Id,
                Name = r.Name
            });
        }

        public async Task<DisplayRoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            return role == null ? null : new DisplayRoleDto
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public async Task<DisplayRoleDto> CreateRoleAsync(string name)
        {
            var role = new Role(name);
            await _roleRepository.AddAsync(role);
            
            return new DisplayRoleDto
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public async Task UpdateRoleAsync(int id, string name)
        {
            var role = await _roleRepository.GetByIdAsync(id);
            if (role == null)
                throw new ArgumentException($"Роль с id = {id} не найдена.");

            role.Name = name ?? throw new ArgumentNullException(nameof(name));
            await _roleRepository.UpdateAsync(role);
        }

        public async Task DeleteRoleAsync(int id)
        {
            await _roleRepository.DeleteAsync(id);
        }
    }
}