using Microsoft.AspNetCore.Mvc;
using Services.Statuses;
using Services.DTO.Status;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    
    private readonly IStatusService _statusService;

    public StatusController(IStatusService statusService)
    {
        _statusService = statusService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DisplayStatusDto>>> GetAllStatuses()
    {
        var statuses = await _statusService.GetAllStatusesAsync();
        return Ok(statuses.ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DisplayStatusDto>> GetStatusById(int id)
    {
        var status = await _statusService.GetStatusByIdAsync(id);
        return status == null ? NotFound() : Ok(status);
    }

    [HttpPost]
    public async Task<ActionResult<DisplayStatusDto>> CreateStatus([FromBody] string name)
    {
        var status = await _statusService.CreateStatusAsync(name);
        return CreatedAtAction(nameof(GetStatusById), new { id = status.Id }, status);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateStatus(int id, [FromBody] string name)
    {
        await _statusService.UpdateStatusAsync(id, name);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteStatus(int id)
    {
        await _statusService.DeleteStatusAsync(id);
        return NoContent();
    }
}