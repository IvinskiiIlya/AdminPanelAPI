using Microsoft.AspNetCore.Identity;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Services.DTO.Filtration;
using Services.DTO.Pagination;
using Services.DTO.User;

namespace Services.Users
{
    public class UserService : IUserService
    {
        
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<PagedResponse<DisplayUserDto>> GetAllUsersAsync(UserFilterParams filters)
        {
            var query = _userManager.Users.AsQueryable();
            
            if (!string.IsNullOrEmpty(filters.SearchTerm))
                query = query.Where(u => 
                    u.Name.Contains(filters.SearchTerm) ||
                    u.Email.Contains(filters.SearchTerm) ||
                    u.Id.ToString().Contains(filters.SearchTerm)
                );
            else
            {
                if (!string.IsNullOrEmpty(filters.Name))
                    query = query.Where(u => u.Name.Contains(filters.Name));
                if (!string.IsNullOrEmpty(filters.Email))
                    query = query.Where(u => u.Email.Contains(filters.Email));
            }
            if (filters.CreatedFrom.HasValue)
                query = query.Where(u => u.CreatedAt >= filters.CreatedFrom);
            if (filters.CreatedTo.HasValue)
                query = query.Where(u => u.CreatedAt <= filters.CreatedTo);

            var totalRecords = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.Id)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            var userDtos = users.Select(u => new DisplayUserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                CreatedAt = u.CreatedAt,
                LastLogin = u.LastLogin
            }).ToList();
            
            return new PagedResponse<DisplayUserDto>(
                userDtos,
                filters.PageNumber,
                filters.PageSize,
                totalRecords
            );
        }

        public async Task<DisplayUserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            return user == null ? null : new DisplayUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin
            };
        }

        public async Task<DisplayUserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new User(createUserDto.Name, createUserDto.Email)
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
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin
            };
        }

        public async Task UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ArgumentException($"Пользователь с id = {id} не найден.");

            user.Name = updateUserDto.Name ?? user.Name;
            user.Email = updateUserDto.Email ?? user.Email;
            user.UserName = updateUserDto.Email ?? user.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ArgumentException($"Пользователь с id = {id} не найден.");

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }
}