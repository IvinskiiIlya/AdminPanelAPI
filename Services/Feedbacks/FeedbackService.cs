using Repositories.Feedbacks;
using Data.Models;
using Services.DTO.Feedback;

namespace Services.Feedbacks
{
    public class FeedbackService : IFeedbackService
    {
        
        private readonly IFeedbackRepository _feedbackRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository)
        {
            _feedbackRepository = feedbackRepository;
        }

        public async Task<IEnumerable<DisplayFeedbackDto>> GetAllFeedbacksAsync()
        {
            var feedbacks = await _feedbackRepository.GetAllAsync();
            return feedbacks.Select(f => new DisplayFeedbackDto
            {
                Id = f.Id,
                Message = f.Message,
                StatusId = f.StatusId, 
                CategoryId = f.CategoryId, 
                UserId = f.UserId,
                CreatedAt = f.CreatedAt
            });
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

            await _feedbackRepository.UpdateAsync(feedback);
        }

        public async Task DeleteFeedbackAsync(int id)
        {
            await _feedbackRepository.DeleteAsync(id);
        }
    }
}