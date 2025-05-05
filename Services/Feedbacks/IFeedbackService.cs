using System.Collections.Generic;
using System.Threading.Tasks;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

namespace Services.Feedbacks
{
    public interface IFeedbackService
    {
        Task<IEnumerable<DisplayFeedbackDto>> GetAllFeedbacksAsync();
        Task<DisplayFeedbackDto?> GetFeedbackByIdAsync(int id);
        Task<DisplayFeedbackDto> CreateFeedbackAsync(CreateFeedbackDto createFeedbackDto); // Изменен возвращаемый тип
        Task UpdateFeedbackAsync(int id, UpdateFeedbackDto updateFeedbackDto); // Добавлен параметр id
        Task DeleteFeedbackAsync(int id);
    }
}