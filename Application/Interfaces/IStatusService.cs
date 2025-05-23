using Application.DTO;
using Application.DTO.Status;

namespace Application.Interfaces
{
    public interface IStatusService
    {
        Task<PagedResponse<DisplayStatusDto>> GetAllStatusesAsync(FilterStatusDto filters);
        Task<DisplayStatusDto?> GetStatusByIdAsync(int id);
        Task<DisplayStatusDto> CreateStatusAsync(string name);
        Task UpdateStatusAsync(int id, string name);
        Task DeleteStatusAsync(int id);
    }
}