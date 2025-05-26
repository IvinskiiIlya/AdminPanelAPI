using Application.DTO;
using Application.DTO.Role;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class RoleService : IRoleService
    {
        
        private readonly RoleManager<Role> _roleManager;

        public RoleService(RoleManager<Role> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<PagedResponse<DisplayRoleDto>> GetAllRolesAsync(FilterRoleDto filters)
        {
            var roles = await _roleManager.Roles.ToListAsync();
            var filtered = roles.AsQueryable();

            if (!string.IsNullOrEmpty(filters.SearchTerm))
                filtered = filtered.Where(r => r.Name.Contains(filters.SearchTerm, StringComparison.OrdinalIgnoreCase));
            else
                if (!string.IsNullOrEmpty(filters.Name))
                    filtered = filtered.Where(r => r.Name.Contains(filters.Name, StringComparison.OrdinalIgnoreCase));


            var totalRecords = filtered.Count();

            var paged = filtered
                .OrderBy(r => r.Id)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            var roleDtos = paged.Select(r => new DisplayRoleDto
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            return new PagedResponse<DisplayRoleDto>(
                roleDtos,
                filters.PageNumber,
                filters.PageSize,
                totalRecords
            );
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

        public async Task UpdateRoleAsync(int id, UpdateRoleDto updateRoleDto)
        {
            var role = await _roleManager.FindByIdAsync(id.ToString());
            if (role == null)
                throw new ArgumentException($"Роль с id = {id} не найдена.");

            role.Name = updateRoleDto.Name;
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