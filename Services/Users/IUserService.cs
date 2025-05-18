using Services.DTO.User;

namespace Services.Users
{
    public interface IUserService
    {
        Task<IEnumerable<DisplayUserDto>> GetAllUsersAsync();
        Task<DisplayUserDto?> GetUserByIdAsync(int id);
        Task<DisplayUserDto> CreateUserAsync(CreateUserDto createUserDto);
        Task UpdateUserAsync(int id, UpdateUserDto updateUserDto); 
        Task DeleteUserAsync(int id);
    }
}