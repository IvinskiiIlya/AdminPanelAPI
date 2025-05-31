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
        
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<PagedResponse<DisplayRoleDto>> GetAllRolesAsync(FilterRoleDto filters)
        {
            var query = _roleManager.Roles.AsQueryable();

            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(r => r.Name.Contains(filters.Name));

            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var search = filters.SearchTerm.ToLower();
                query = query.Where(r => 
                    r.Name.ToLower().Contains(search)
                );
            }
            
            query = ApplySorting(query, filters.SortColumn, filters.SortOrder);

            var totalRecords = await query.CountAsync();
            var paged = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            var dtos = paged.Select(r => new DisplayRoleDto
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();

            return new PagedResponse<DisplayRoleDto>(
                dtos,
                filters.PageNumber,
                filters.PageSize,
                totalRecords
            );
        }

        private IQueryable<IdentityRole> ApplySorting(IQueryable<IdentityRole> query, string? sortColumn, string? sortOrder)
        {
            sortColumn = (string.IsNullOrEmpty(sortColumn) ? "Name" : sortColumn).Trim();
            sortOrder = (string.IsNullOrEmpty(sortOrder) ? "asc" : sortOrder).Trim().ToLower();

            var validColumns = new[] { "Id", "Name" };
            if (!validColumns.Contains(sortColumn, StringComparer.OrdinalIgnoreCase))
                sortColumn = "Name";

            return sortOrder == "desc" 
                ? query.OrderByDescending(r => EF.Property<object>(r, sortColumn)) 
                : query.OrderBy(r => EF.Property<object>(r, sortColumn));
        }

        public async Task<DisplayRoleDto?> GetRoleByIdAsync(string id)
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
            var role = new IdentityRole();
            role.Name = name;
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

        public async Task UpdateRoleAsync(string id, UpdateRoleDto updateRoleDto)
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

        public async Task DeleteRoleAsync(string id)
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