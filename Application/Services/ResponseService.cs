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
            var query = responses.AsQueryable();

            if (filters.CreatedFrom.HasValue)
                query = query.Where(r => r.CreatedAt >= filters.CreatedFrom.Value);

            if (filters.CreatedTo.HasValue)
                query = query.Where(r => r.CreatedAt <= filters.CreatedTo.Value);

            if (filters.FeedbackId.HasValue)
                query = query.Where(r => r.FeedbackId == filters.FeedbackId.Value);

            if (!string.IsNullOrEmpty(filters.UserId))
            {
                var userIdFilter = filters.UserId.ToLower();
                query = query.Where(r => r.UserId != null && r.UserId.ToLower().Contains(userIdFilter));
            }

            if (!string.IsNullOrEmpty(filters.Message))
            {
                var messageFilter = filters.Message.ToLower();
                query = query.Where(r => r.Message != null && r.Message.ToLower().Contains(messageFilter));
            }

            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var search = filters.SearchTerm.ToLower();
                query = query.Where(r =>
                    (!string.IsNullOrEmpty(r.Message) && r.Message.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(r.UserId) && r.UserId.ToLower().Contains(search))
                );
            }

            var totalRecords = query.Count();

            query = ApplySorting(query, filters.SortColumn, filters.SortOrder);

            var paged = query
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

        private IQueryable<Response> ApplySorting(IQueryable<Response> query, string? sortColumn, string? sortOrder)
        {
            if (string.IsNullOrEmpty(sortColumn))
                sortColumn = "Id";
            
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "asc";

            sortColumn = sortColumn.ToLower();
            sortOrder = sortOrder.ToLower();

            return sortColumn switch
            {
                "id" => sortOrder == "desc" ? query.OrderByDescending(r => r.Id) : query.OrderBy(r => r.Id),
                "feedbackid" => sortOrder == "desc" ? query.OrderByDescending(r => r.FeedbackId) : query.OrderBy(r => r.FeedbackId),
                "userid" => sortOrder == "desc" ? query.OrderByDescending(r => r.UserId) : query.OrderBy(r => r.UserId),
                "message" => sortOrder == "desc" ? query.OrderByDescending(r => r.Message) : query.OrderBy(r => r.Message),
                "createdat" => sortOrder == "desc" ? query.OrderByDescending(r => r.CreatedAt) : query.OrderBy(r => r.CreatedAt),
                _ => query.OrderBy(r => r.Id)
            };
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
            response.CreatedAt = DateTime.SpecifyKind(response.CreatedAt, DateTimeKind.Utc);

            await _responseRepository.UpdateAsync(response);
        }

        public async Task DeleteResponseAsync(int id)
        {
            await _responseRepository.DeleteAsync(id);
        }
    }
}