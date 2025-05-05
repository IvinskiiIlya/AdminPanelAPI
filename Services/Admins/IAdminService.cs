using System.Collections;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

namespace Services.Admins
{
    public interface IAdminService
    {
        Task<IEnumerable<DisplayAdminDto>> GetAllAdminsAsync(); // Указан конкретный тип возвращаемого значения
        Task<DisplayAdminDto?> GetAdminByIdAsync(int id);
        Task<DisplayAdminDto> AddAdminAsync(CreateAdminDto createAdminDto); // Возвращает DTO
        Task UpdateAdminAsync(int id, UpdateAdminDto updateAdminDto); // Добавлен параметр id
        Task DeleteAdminAsync(int id);
    }
}