using Application.DTO;
using Application.DTO.Response;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class ResponseService : IResponseService
    {
        
        private readonly IResponseRepository _responseRepository;

        public ResponseService(IResponseRepository responseRepository)
        {
            _responseRepository = responseRepository;
        }
        
        public async Task<PagedResponse<DisplayResponseDto>> GetAllResponsesAsync(FilterResponseDto filters)
        {
            var responses = await _responseRepository.GetAllAsync();
            var filtered = responses.AsQueryable();

            if (!string.IsNullOrEmpty(filters.SearchTerm))
                filtered = filtered.Where(r => r.Message.Contains(filters.SearchTerm));
            else
            {
                if (!string.IsNullOrEmpty(filters.Message))
                    filtered = filtered.Where(r => r.Message.Contains(filters.Message));

                if (filters.UserId.HasValue)
                    filtered = filtered.Where(r => r.UserId == filters.UserId.Value);

                if (filters.FeedbackId.HasValue)
                    filtered = filtered.Where(r => r.FeedbackId == filters.FeedbackId.Value);
            }

            var totalRecords = filtered.Count();

            var paged = filtered
                .OrderBy(r => r.Id)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            var dtos = paged.Select(r => new DisplayResponseDto
            {
                Id = r.Id,
                FeedbackId = r.FeedbackId,
                UserId = r.UserId,
                Message = r.Message,
                CreatedAt = r.CreatedAt
            }).ToList();

            return new PagedResponse<DisplayResponseDto>(
                dtos,
                filters.PageNumber,
                filters.PageSize,
                totalRecords
            );
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

        public async Task DeleteResponseAsync(int id)
        {
            await _responseRepository.DeleteAsync(id);
        }
    }
}