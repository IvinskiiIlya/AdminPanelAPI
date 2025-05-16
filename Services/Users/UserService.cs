using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Users;
using Data.Models;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

namespace Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<DisplayUserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(u => new DisplayUserDto
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<DisplayUserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : new DisplayUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<DisplayUserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new User
            {
                UserName = createUserDto.UserName,
                Email = createUserDto.Email,
                CreatedAt = DateTime.UtcNow
            };
            
            await _userRepository.AddAsync(user);
            
            return new DisplayUserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new ArgumentException($"User with id {id} not found.");
            }

            user.UserName = updateUserDto.UserName!;
            user.Email = updateUserDto.Email!;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }
    }
}