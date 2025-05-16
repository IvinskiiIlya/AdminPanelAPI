using System.Collections.Generic;
using System.Threading.Tasks;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

namespace Services.Users
{
    public interface IUserService
    {
        Task<IEnumerable<DisplayUserDto>> GetAllUsersAsync();
        Task<DisplayUserDto?> GetUserByIdAsync(int id);
        Task<DisplayUserDto> CreateUserAsync(CreateUserDto createUserDto); // Изменен возвращаемый тип
        Task UpdateUserAsync(int id, UpdateUserDto updateUserDto); // Добавлен параметр id
        Task DeleteUserAsync(int id);
    }
}