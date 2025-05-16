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
        private readonly IAdminService _adminService; 

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost]
        public async Task<ActionResult<DisplayAdminDto>> CreateAdmin([FromBody] CreateAdminDto dto)
        {
            var admin = await _adminService.AddAdminAsync(dto); 
            return CreatedAtAction(
                nameof(GetAdminById), 
                new { id = admin.Id },
                admin 
            );
        }

        [HttpPut("{id}")]
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