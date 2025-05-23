using Application.DTO;
using Application.DTO.Response;

namespace Application.Interfaces
{
    public interface IResponseService
    {
        Task<PagedResponse<DisplayResponseDto>> GetAllResponsesAsync(FilterResponseDto filters);
        Task<DisplayResponseDto?> GetResponseByIdAsync(int id);
        Task<List<DisplayResponseDto>> GetResponsesByFeedbackAsync(int feedbackId); 
        Task<DisplayResponseDto> CreateResponseAsync(CreateResponseDto createResponseDto); 
        Task UpdateResponseAsync(int id, UpdateResponseDto updateResponseDto);
        Task DeleteResponseAsync(int id);
    }
}