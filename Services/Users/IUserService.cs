using Services.DTO.Filtration;
using Services.DTO.Pagination;
using Services.DTO.User;

namespace Services.Users
{
    public interface IUserService
    {
        Task<PagedResponse<DisplayUserDto>> GetAllUsersAsync(UserFilterParams filters);
        Task<DisplayUserDto?> GetUserByIdAsync(int id);
        Task<DisplayUserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task UpdateUserAsync(int id, UpdateUserDto updateUserDto); 
        Task DeleteUserAsync(int id);
    }
}