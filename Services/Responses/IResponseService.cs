using System.Collections.Generic;
using System.Threading.Tasks;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

namespace Services.Responses
{
    public interface IResponseService
    {
        Task<List<DisplayResponseDto>> GetResponsesByFeedbackAsync(int feedbackId); // Добавлен новый метод
        Task<DisplayResponseDto> CreateResponseAsync(CreateResponseDto createResponseDto); // Изменен возвращаемый тип
        Task UpdateResponseAsync(int id, UpdateResponseDto updateResponseDto); // Добавлен параметр id
        
        // Остальные методы
        Task<IEnumerable<DisplayResponseDto>> GetAllResponsesAsync();
        Task<DisplayResponseDto?> GetResponseByIdAsync(int id);
        Task DeleteResponseAsync(int id);
    }
}