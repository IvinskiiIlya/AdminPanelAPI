using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Feedbacks;
using Data.Models;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

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
                FeedbackType = f.FeedbackType,
                Message = f.Message,
                Status = f.Status,
                CreatedAt = f.CreatedAt,
                UserId = f.UserId,
                Attachments = new List<DisplayAttachmentDto>(),
                Categories = new List<DisplayCategoryDto>()
            });
        }

        public async Task<DisplayFeedbackDto?> GetFeedbackByIdAsync(int id)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null) return null;
            
            return new DisplayFeedbackDto
            {
                Id = feedback.Id,
                FeedbackType = feedback.FeedbackType,
                Message = feedback.Message,
                Status = feedback.Status,
                CreatedAt = feedback.CreatedAt,
                UserId = feedback.UserId,
                Attachments = new List<DisplayAttachmentDto>(),
                Categories = new List<DisplayCategoryDto>()
            };
        }

        public async Task<DisplayFeedbackDto> CreateFeedbackAsync(CreateFeedbackDto createFeedbackDto)
        {
            var feedback = new Feedback
            {
                FeedbackType = createFeedbackDto.FeedbackType,
                Message = createFeedbackDto.Message,
                UserId = createFeedbackDto.UserId,
                Status = "Новый",
                CreatedAt = DateTime.UtcNow
            };
            
            await _feedbackRepository.AddAsync(feedback);
            
            return new DisplayFeedbackDto
            {
                Id = feedback.Id,
                FeedbackType = feedback.FeedbackType,
                Message = feedback.Message,
                Status = feedback.Status,
                CreatedAt = feedback.CreatedAt,
                UserId = feedback.UserId,
                Attachments = new List<DisplayAttachmentDto>(),
                Categories = new List<DisplayCategoryDto>()
            };
        }

        public async Task UpdateFeedbackAsync(int id, UpdateFeedbackDto updateFeedbackDto)
        {
            var feedback = await _feedbackRepository.GetByIdAsync(id);
            if (feedback == null)
            {
                throw new ArgumentException($"Feedback with id {id} not found.");
            }

            feedback.FeedbackType = updateFeedbackDto.FeedbackType!;
            feedback.Message = updateFeedbackDto.Message!;
            feedback.Status = updateFeedbackDto.Status!;

            await _feedbackRepository.UpdateAsync(feedback);
        }

        public async Task DeleteFeedbackAsync(int id)
        {
            await _feedbackRepository.DeleteAsync(id);
        }
    }
}