using Repositories.Responses;
using Data.Models;
using Services.DTO.Response;

namespace Services.Responses
{
    public class ResponseService : IResponseService
    {
        
        private readonly IResponseRepository _responseRepository;

        public ResponseService(IResponseRepository responseRepository)
        {
            _responseRepository = responseRepository;
        }

        public async Task<List<DisplayResponseDto>> GetResponsesByFeedbackAsync(int feedbackId)
        {
            var responses = await _responseRepository.GetByFeedbackIdAsync(feedbackId);
            return responses.Select(r => new DisplayResponseDto
            {
                Id = r.Id,
                Message = r.Message,  
                CreatedAt = r.CreatedAt,
                FeedbackId = r.FeedbackId,
                UserId = r.UserId    
            }).ToList();
        }

        public async Task<DisplayResponseDto> CreateResponseAsync(CreateResponseDto createResponseDto)
        {
            var response = new Response(createResponseDto.Message)  
            {
                FeedbackId = createResponseDto.FeedbackId,
                UserId = createResponseDto.UserId, 
                CreatedAt = DateTime.UtcNow
            };
            
            await _responseRepository.AddAsync(response);
            
            return new DisplayResponseDto
            {
                Id = response.Id,
                Message = response.Message,  
                CreatedAt = response.CreatedAt,
                FeedbackId = response.FeedbackId,
                UserId = response.UserId     
            };
        }

        public async Task UpdateResponseAsync(int id, UpdateResponseDto updateResponseDto)
        {
            var response = await _responseRepository.GetByIdAsync(id);
            if (response == null)
                throw new ArgumentException($"Ответ с id = {id} не найден.");

            response.Message = updateResponseDto.Message ?? response.Message;  

            await _responseRepository.UpdateAsync(response);
        }

        public async Task<IEnumerable<DisplayResponseDto>> GetAllResponsesAsync()
        {
            var responses = await _responseRepository.GetAllAsync();
            return responses.Select(r => new DisplayResponseDto
            {
                Id = r.Id,
                Message = r.Message,  
                CreatedAt = r.CreatedAt,
                FeedbackId = r.FeedbackId,
                UserId = r.UserId    
            });
        }

        public async Task<DisplayResponseDto?> GetResponseByIdAsync(int id)
        {
            var response = await _responseRepository.GetByIdAsync(id);
            return response == null ? null : new DisplayResponseDto
            {
                Id = response.Id,
                Message = response.Message,  
                CreatedAt = response.CreatedAt,
                FeedbackId = response.FeedbackId,
                UserId = response.UserId   
            };
        }

        public async Task DeleteResponseAsync(int id)
        {
            await _responseRepository.DeleteAsync(id);
        }
    }
}