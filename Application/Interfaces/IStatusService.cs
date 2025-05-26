using Application.DTO;
using Application.DTO.Status;

namespace Application.Interfaces
{
    public interface IStatusService
    {
        Task<PagedResponse<DisplayStatusDto>> GetAllStatusesAsync(FilterStatusDto filters);
        Task<DisplayStatusDto?> GetStatusByIdAsync(int id);
        Task<DisplayStatusDto> CreateStatusAsync(CreateStatusDto createStatusDto);
        Task UpdateStatusAsync(int id, UpdateStatusDto updateStatusDto);
        Task DeleteStatusAsync(int id);
    }
}