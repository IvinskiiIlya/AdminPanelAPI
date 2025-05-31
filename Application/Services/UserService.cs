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
        
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PagedResponse<DisplayUserDto>> GetAllUsersAsync(FilterUserDto filters)
        {
            var query = _userManager.Users.AsQueryable();
            
            if (!string.IsNullOrEmpty(filters.SearchTerm))
                query = query.Where(u => 
                    u.UserName.Contains(filters.SearchTerm) ||
                    u.Email.Contains(filters.SearchTerm) ||
                    u.Id.ToString().Contains(filters.SearchTerm)
                );
            else
            {
                if (!string.IsNullOrEmpty(filters.Name))
                    query = query.Where(u => u.UserName.Contains(filters.Name));
                if (!string.IsNullOrEmpty(filters.Email))
                    query = query.Where(u => u.Email.Contains(filters.Email));
            }

            var totalRecords = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.Id)
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
            var user = new User()
            {
                Email = createUserDto.Email,
                UserName = createUserDto.Email
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
            user.UserName = updateUserDto.Email ?? user.UserName;

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