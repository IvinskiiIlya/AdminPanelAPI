using Services.DTO.Status;

namespace Services.Statuses;

public interface IStatusService
{
    Task<IEnumerable<DisplayStatusDto>> GetAllStatusesAsync();
    Task<DisplayStatusDto?> GetStatusByIdAsync(int id);
    Task<DisplayStatusDto> CreateStatusAsync(string name);
    Task UpdateStatusAsync(int id, string name);
    Task DeleteStatusAsync(int id);
}