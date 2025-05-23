using Application.DTO;
using Application.DTO.Feedback;
using Application.DTO.Response;

namespace Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<PagedResponse<DisplayFeedbackDto>> GetAllFeedbacksAsync(FilterFeedbackDto filters);
        Task<DisplayFeedbackDto?> GetFeedbackByIdAsync(int id);
        Task<DisplayFeedbackDto> CreateFeedbackAsync(CreateFeedbackDto createFeedbackDto); 
        Task UpdateFeedbackAsync(int id, UpdateFeedbackDto updateFeedbackDto); 
        Task DeleteFeedbackAsync(int id);
    }
}