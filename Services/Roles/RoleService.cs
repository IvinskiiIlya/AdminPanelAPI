using Microsoft.AspNetCore.Identity;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Services.DTO.Role;

namespace Services.Roles
{
    public class RoleService : IRoleService
    {
        
        private readonly RoleManager<Role> _roleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<DisplayRoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(r => new DisplayRoleDto
            {
                Id = r.Id,
                Name = r.Name
            });
        }

        public async Task<DisplayRoleDto?> GetRoleByIdAsync(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            return role == null ? null : new DisplayRoleDto
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public async Task<DisplayRoleDto> CreateRoleAsync(string name)
        {
            var role = new Role(name);
            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            return new DisplayRoleDto
            {
                Id = role.Id,
                Name = role.Name
            };
        }

        public async Task UpdateRoleAsync(int id, string name)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
                throw new ArgumentException($"Роль с id = {id} не найдена.");

            role.Name = name;
            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        public async Task DeleteRoleAsync(int id)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
                throw new ArgumentException($"Роль с id = {id} не найдена.");

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}