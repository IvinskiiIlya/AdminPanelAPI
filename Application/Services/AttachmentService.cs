using Application.DTO.Attachment;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class AttachmentService : IAttachmentService
    {
        
        private readonly IAttachmentRepository _attachmentRepository;

        public AttachmentService(IAttachmentRepository attachmentRepository)
        {
            _attachmentRepository = attachmentRepository;
        }

        public async Task<IEnumerable<DisplayAttachmentDto>> GetAllAttachmentsAsync()
        {
            var attachments = await _attachmentRepository.GetAllAsync();
            return attachments.Select(a => new DisplayAttachmentDto
            {
                Id = a.Id,
                FeedbackId = a.FeedbackId,
                FilePath = a.FilePath,
                FileType = a.FileType,
                CreatedAt = a.CreatedAt
            });
        }

        public async Task<List<DisplayAttachmentDto>> GetAttachmentsByFeedbackAsync(int feedbackId)
        {
            var attachments = await _attachmentRepository.GetByFeedbackIdAsync(feedbackId);
            return attachments.Select(a => new DisplayAttachmentDto
            {
                Id = a.Id,
                FeedbackId = a.FeedbackId,
                FilePath = a.FilePath,
                FileType = a.FileType,
                CreatedAt = a.CreatedAt
            }).ToList();
        }

        public async Task<DisplayAttachmentDto?> GetAttachmentByIdAsync(int id)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(id);
            if (attachment == null) return null;
            
            return new DisplayAttachmentDto
            {
                Id = attachment.Id,
                FeedbackId = attachment.FeedbackId,
                FilePath = attachment.FilePath,
                FileType = attachment.FileType,
                CreatedAt = attachment.CreatedAt
            };
        }

        public async Task<DisplayAttachmentDto> CreateAttachmentAsync(CreateAttachmentDto createAttachmentDto)
        {
            var attachment = new Attachment
            {
                FeedbackId = createAttachmentDto.FeedbackId,
                FilePath = createAttachmentDto.FilePath,
                FileType = createAttachmentDto.FileType,
                CreatedAt = DateTime.UtcNow
            };
            
            await _attachmentRepository.AddAsync(attachment);
            
            return new DisplayAttachmentDto
            {
                Id = attachment.Id,
                FeedbackId = attachment.FeedbackId,
                FilePath = attachment.FilePath,
                FileType = attachment.FileType,
                CreatedAt = attachment.CreatedAt
            };
        }

        public async Task UpdateAttachmentAsync(UpdateAttachmentDto updateAttachmentDto)
        {
            var attachment = await _attachmentRepository.GetByIdAsync(updateAttachmentDto.Id);
            if (attachment == null)
                throw new ArgumentException($"Вложение с id = {updateAttachmentDto.Id} не найдено.");

            attachment.FilePath = updateAttachmentDto.FilePath;
            attachment.FileType = updateAttachmentDto.FileType;

            await _attachmentRepository.UpdateAsync(attachment);
        }

        public async Task DeleteAttachmentAsync(int id)
        {
            await _attachmentRepository.DeleteAsync(id);
        }
    }
}