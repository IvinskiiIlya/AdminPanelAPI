using Application.DTO;
using Application.DTO.Feedback;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<PagedResponse<DisplayFeedbackDto>> GetAllFeedbacksAsync(FilterFeedbackDto filters)
        {
            var feedbacks = await _feedbackRepository.GetAllAsync();
            var query = feedbacks.AsQueryable();

            if (filters.CreatedFrom.HasValue)
                query = query.Where(f => f.CreatedAt >= filters.CreatedFrom.Value);

            if (filters.CreatedTo.HasValue)
                query = query.Where(f => f.CreatedAt <= filters.CreatedTo.Value);

            if (!string.IsNullOrEmpty(filters.UserId))
            {
                var userIdFilter = filters.UserId.ToLower();
                query = query.Where(f => f.UserId != null && f.UserId.ToLower().Contains(userIdFilter));
            }

            if (filters.CategoryId.HasValue)
                query = query.Where(f => f.CategoryId == filters.CategoryId.Value);

            if (filters.StatusId.HasValue)
                query = query.Where(f => f.StatusId == filters.StatusId.Value);

            if (!string.IsNullOrEmpty(filters.Message))
            {
                var messageFilter = filters.Message.ToLower();
                query = query.Where(f => f.Message != null && f.Message.ToLower().Contains(messageFilter));
            }

            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var search = filters.SearchTerm.ToLower();
                query = query.Where(f =>
                    (!string.IsNullOrEmpty(f.Message) && f.Message.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(f.UserId) && f.UserId.ToLower().Contains(search))
                );
            }

            var totalRecords = query.Count();

            query = ApplySorting(query, filters.SortColumn, filters.SortOrder);

            var paged = query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            var dtos = paged.Select(f => new DisplayFeedbackDto
            {
                Id = f.Id,
                Message = f.Message,
                StatusId = f.StatusId,
                CategoryId = f.CategoryId,
                UserId = f.UserId,
                CreatedAt = f.CreatedAt
            }).ToList();

            return new PagedResponse<DisplayFeedbackDto>(
                dtos,
                filters.PageNumber,
                filters.PageSize,
                totalRecords
            );
        }

        private IQueryable<Feedback> ApplySorting(IQueryable<Feedback> query, string? sortColumn, string? sortOrder)
        {
            if (string.IsNullOrEmpty(sortColumn))
                sortColumn = "Id";
            
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "asc";

            sortColumn = sortColumn.ToLower();
            sortOrder = sortOrder.ToLower();

            return sortColumn switch
            {
                "id" => sortOrder == "desc" ? query.OrderByDescending(f => f.Id) : query.OrderBy(f => f.Id),
                "userid" => sortOrder == "desc" ? query.OrderByDescending(f => f.UserId) : query.OrderBy(f => f.UserId),
                "categoryid" => sortOrder == "desc" ? query.OrderByDescending(f => f.CategoryId) : query.OrderBy(f => f.CategoryId),
                "statusid" => sortOrder == "desc" ? query.OrderByDescending(f => f.StatusId) : query.OrderBy(f => f.StatusId),
                "message" => sortOrder == "desc" ? query.OrderByDescending(f => f.Message) : query.OrderBy(f => f.Message),
                "createdat" => sortOrder == "desc" ? query.OrderByDescending(f => f.CreatedAt) : query.OrderBy(f => f.CreatedAt),
                _ => query.OrderBy(f => f.Id)
            };
        }
        
        public async Task<DisplayFeedbackDto?> GetFeedbackByIdAsync(int id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null) return null;
            
            return new DisplayFeedbackDto
            {
                Id = feedback.Id,
                Message = feedback.Message,
                StatusId = feedback.StatusId,
                CategoryId = feedback.CategoryId,
                UserId = feedback.UserId,
                CreatedAt = feedback.CreatedAt
            };
        }

        public async Task<DisplayFeedbackDto> CreateFeedbackAsync(CreateFeedbackDto createFeedbackDto)
        {
            var feedback = new Feedback(createFeedbackDto.Message)
            {
                UserId = createFeedbackDto.UserId,
                CategoryId = createFeedbackDto.CategoryId,
                StatusId = 1 
            };
            
            await _feedbackRepository.AddAsync(feedback);
            
            return new DisplayFeedbackDto
            {
                Id = feedback.Id,
                Message = feedback.Message,
                StatusId = feedback.StatusId,
                CategoryId = feedback.CategoryId,
                UserId = feedback.UserId,
                CreatedAt = feedback.CreatedAt
            };
        }

        public async Task UpdateFeedbackAsync(int id, UpdateFeedbackDto updateFeedbackDto)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null)
                throw new ArgumentException($"Отзыв с id = {id} не найден.");

            feedback.Message = updateFeedbackDto.Message ?? feedback.Message;
            if (updateFeedbackDto.StatusId.HasValue)
                feedback.StatusId = updateFeedbackDto.StatusId.Value;
            if (updateFeedbackDto.CategoryId.HasValue)
                feedback.CategoryId = updateFeedbackDto.CategoryId.Value;
            feedback.CreatedAt = DateTime.SpecifyKind(feedback.CreatedAt, DateTimeKind.Utc);

            await _feedbackRepository.UpdateAsync(feedback);
        }

        public async Task DeleteFeedbackAsync(int id)
        {
            await _feedbackRepository.DeleteAsync(id);
        }
    }
}