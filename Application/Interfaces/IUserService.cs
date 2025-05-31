using Application.DTO;
using Application.DTO.User;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponse<DisplayUserDto>> GetAllUsersAsync(FilterUserDto filters);
        Task<DisplayUserDto?> GetUserByIdAsync(string id);
        Task<DisplayUserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task UpdateUserAsync(string id, UpdateUserDto updateUserDto); 
        Task DeleteUserAsync(string id);
    }
}