using System.Collections;
using Repositories.Admins;
using Data.Models;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

namespace Services.Admins
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<IEnumerable<DisplayAdminDto>> GetAllAdminsAsync()
        {
            var admins = await _adminRepository.GetAllAsync();
            return admins.Select(a => new DisplayAdminDto
            {
                Id = a.Id,
                UserName = a.UserName,
                Email = a.Email,
                CreatedAt = a.CreatedAt
            });
        }

        public async Task<DisplayAdminDto?> GetAdminByIdAsync(int id)
        {
            var admin = await _adminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                return null;
            }
            return new DisplayAdminDto
            {
                Id = admin.Id,
                UserName = admin.UserName,
                Email = admin.Email,
                CreatedAt = admin.CreatedAt
            };
        }

        public async Task<DisplayAdminDto> AddAdminAsync(CreateAdminDto createAdminDto)
        {
            var admin = new Admin
            {
                UserName = createAdminDto.UserName,
                Email = createAdminDto.Email,
                CreatedAt = DateTime.UtcNow
            };
    
            await _adminRepository.AddAsync(admin);
    
            return new DisplayAdminDto
            {
                Id = admin.Id,
                UserName = admin.UserName,
                Email = admin.Email,
                CreatedAt = admin.CreatedAt
            };
        }

        public async Task UpdateAdminAsync(int id, UpdateAdminDto updateAdminDto) // Добавлен параметр id
        {
            var admin = await _adminRepository.GetByIdAsync(id);
            if (admin == null)
            {
                throw new ArgumentException($"Admin with id {id} not found.");
            }

            admin.UserName = updateAdminDto.UserName!; // ! так как [Required] гарантирует не-null
            admin.Email = updateAdminDto.Email!;
    
            await _adminRepository.UpdateAsync(admin);
        }

        public async Task DeleteAdminAsync(int id)
        {
            await _adminRepository.DeleteAsync(id);
        }
    }
}