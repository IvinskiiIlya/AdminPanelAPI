using Repositories.Users;
using Data.Models;
using Services.DTO.User;

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
                Name = u.Name,  
                Email = u.Email,
                RoleId = u.RoleId,  
                CreatedAt = u.CreatedAt,
                LastLogin = u.LastLogin  
            });
        }

        public async Task<DisplayUserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user == null ? null : new DisplayUserDto
            {
                Id = user.Id,
                Name = user.Name,  
                Email = user.Email,
                RoleId = user.RoleId,  
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin  
            };
        }

        public async Task<DisplayUserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new User(
                name: createUserDto.Name,  
                email: createUserDto.Email,
                roleId: createUserDto.RoleId  
            );
            
            await _userRepository.AddAsync(user);
            
            return new DisplayUserDto
            {
                Id = user.Id,
                Name = user.Name,  
                Email = user.Email,
                RoleId = user.RoleId, 
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin  
            };
        }

        public async Task UpdateUserAsync(int id, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new ArgumentException($"Пользователь с id = {id} не найден.");

            user.Name = updateUserDto.Name ?? user.Name; 
            user.Email = updateUserDto.Email ?? user.Email;
            
            if (updateUserDto.RoleId.HasValue)
                user.RoleId = updateUserDto.RoleId.Value;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }
    }
}