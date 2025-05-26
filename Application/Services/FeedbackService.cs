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
            var filtered = feedbacks.AsQueryable();

            if (!string.IsNullOrEmpty(filters.SearchTerm))
                filtered = filtered.Where(f => f.Message.Contains(filters.SearchTerm));
            else
            {
                if (!string.IsNullOrEmpty(filters.Message))
                    filtered = filtered.Where(f => f.Message.Contains(filters.Message));

                if (filters.StatusId.HasValue)
                    filtered = filtered.Where(f => f.StatusId == filters.StatusId.Value);

                if (filters.CategoryId.HasValue)
                    filtered = filtered.Where(f => f.CategoryId == filters.CategoryId.Value);

                if (filters.UserId.HasValue)
                    filtered = filtered.Where(f => f.UserId == filters.UserId.Value);
            }

            var totalRecords = filtered.Count();

            var paged = filtered
                .OrderBy(f => f.Id)
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