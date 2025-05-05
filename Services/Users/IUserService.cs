using System.Collections;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

namespace Services.Users
{
    public interface IUserService
    {
        Task<IEnumerable> GetAllUsersAsync();
        Task<DisplayUserDto?> GetUserByIdAsync(int id);
        Task AddUserAsync(CreateUserDto createUserDto);
        Task UpdateUserAsync(UpdateUserDto updateUserDto);
        Task DeleteUserAsync(int id);
    }
}