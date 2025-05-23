using Application.DTO.Response;

namespace Application.Interfaces
{
    public interface IResponseService
    {
        Task<List<DisplayResponseDto>> GetResponsesByFeedbackAsync(int feedbackId); 
        Task<DisplayResponseDto> CreateResponseAsync(CreateResponseDto createResponseDto); 
        Task UpdateResponseAsync(int id, UpdateResponseDto updateResponseDto);
        Task<IEnumerable<DisplayResponseDto>> GetAllResponsesAsync();
        Task<DisplayResponseDto?> GetResponseByIdAsync(int id);
        Task DeleteResponseAsync(int id);
    }
}