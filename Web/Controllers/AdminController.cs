using Services.Admins;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService; // Пробел между типом и именем

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost]
        public async Task<ActionResult<DisplayAdminDto>> CreateAdmin([FromBody] CreateAdminDto dto)
        {
            var admin = await _adminService.AddAdminAsync(dto); // Пробел после await
            return CreatedAtAction(
                nameof(GetAdminById), // Указание имени метода для Location header
                new { id = admin.Id }, // Параметры маршрута
                admin // Возвращаемый объект
            );
        }

        [HttpPut("{id}")] // Исправленный синтаксис атрибута
        public async Task<ActionResult> UpdateAdmin(int id, [FromBody] UpdateAdminDto dto)
        {
            await _adminService.UpdateAdminAsync(id, dto);
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DisplayAdminDto>> GetAdminById(int id)
        {
            var admin = await _adminService.GetAdminByIdAsync(id);
            return Ok(admin);
        }
    }
}