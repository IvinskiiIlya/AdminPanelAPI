using Application.DTO;
using Application.DTO.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponse<DisplayUserDto>> GetAllUsersAsync(FilterUserDto filters);
        Task<DisplayUserDto?> GetUserByIdAsync(int id);
        Task<DisplayUserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task UpdateUserAsync(int id, UpdateUserDto updateUserDto); 
        Task DeleteUserAsync(int id);
    }
}