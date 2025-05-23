using Application.DTO.Feedback;

namespace Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<IEnumerable<DisplayFeedbackDto>> GetAllFeedbacksAsync();
        Task<DisplayFeedbackDto?> GetFeedbackByIdAsync(int id);
        Task<DisplayFeedbackDto> CreateFeedbackAsync(CreateFeedbackDto createFeedbackDto); 
        Task UpdateFeedbackAsync(int id, UpdateFeedbackDto updateFeedbackDto); 
        Task DeleteFeedbackAsync(int id);
    }
}