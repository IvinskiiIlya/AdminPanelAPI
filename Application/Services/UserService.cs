using Application.DTO;
using Application.DTO.User;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService : IUserService
    {
        
        private readonly UserManager<IdentityUser> _userManager;

        public UserService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PagedResponse<DisplayUserDto>> GetAllUsersAsync(FilterUserDto filters)
        {
            var query = _userManager.Users.AsQueryable();

            if (!string.IsNullOrEmpty(filters.Name))
                query = query.Where(u => u.UserName.Contains(filters.Name, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filters.Email))
                query = query.Where(u => u.Email.Contains(filters.Email, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var search = filters.SearchTerm.ToLower();
                query = query.Where(u =>
                    (!string.IsNullOrEmpty(u.UserName) && u.UserName.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(u.Email) && u.Email.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(u.Id) && u.Id.ToLower().Contains(search))
                );
            }

            var totalRecords = await query.CountAsync();

            query = ApplySorting(query, filters.SortColumn, filters.SortOrder);

            var users = await query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            var userDtos = users.Select(u => new DisplayUserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email
            }).ToList();

            return new PagedResponse<DisplayUserDto>(
                userDtos,
                filters.PageNumber,
                filters.PageSize,
                totalRecords
            );
        }

        private IQueryable<IdentityUser> ApplySorting(IQueryable<IdentityUser> query, string? sortColumn, string? sortOrder)
        {
            sortColumn = string.IsNullOrEmpty(sortColumn) ? "UserName" : sortColumn.Trim();
            sortOrder = string.IsNullOrEmpty(sortOrder) ? "asc" : sortOrder.Trim().ToLower();

            var validColumns = new[] { "Id", "UserName", "Email" };
            if (!validColumns.Contains(sortColumn, StringComparer.OrdinalIgnoreCase))
                sortColumn = "UserName";

            return sortOrder == "desc"
                ? query.OrderByDescending(u => EF.Property<object>(u, sortColumn))
                : query.OrderBy(u => EF.Property<object>(u, sortColumn));
        }

        public async Task<DisplayUserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            return user == null ? null : new DisplayUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task<DisplayUserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new IdentityUser()
            {
                Email = createUserDto.Email,
                UserName = createUserDto.UserName
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return new DisplayUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };
        }

        public async Task UpdateUserAsync(string id, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new ArgumentException($"Пользователь с id = {id} не найден.");

            user.UserName = updateUserDto.UserName ?? user.UserName;
            user.Email = updateUserDto.Email ?? user.Email;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                throw new ArgumentException($"Пользователь с id = {id} не найден.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}