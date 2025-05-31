using Application.DTO;
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

        public async Task<PagedResponse<DisplayAttachmentDto>> GetAllAttachmentsAsync(FilterAttachmentDto filters)
        {
            var attachments = await _attachmentRepository.GetAllAsync();
            var query = attachments.AsQueryable();

            if (filters.FeedbackId.HasValue)
                query = query.Where(a => a.FeedbackId == filters.FeedbackId.Value);
            
            if (!string.IsNullOrEmpty(filters.FileType))
                query = query.Where(a => a.FileType.Contains(filters.FileType, StringComparison.OrdinalIgnoreCase));
            
            if (filters.CreatedFrom.HasValue)
                query = query.Where(a => a.CreatedAt >= filters.CreatedFrom.Value);
            
            if (filters.CreatedTo.HasValue)
                query = query.Where(a => a.CreatedAt <= filters.CreatedTo.Value);

            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var search = filters.SearchTerm.ToLower();
                query = query.Where(a =>
                    (!string.IsNullOrEmpty(a.FilePath) && a.FilePath.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(a.FileType) && a.FileType.ToLower().Contains(search))
                );
            }

            var totalRecords = query.Count();
            
            query = ApplySorting(query, filters.SortColumn, filters.SortOrder);

            var paged = query
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            var dtos = paged.Select(a => new DisplayAttachmentDto
            {
                Id = a.Id,
                FeedbackId = a.FeedbackId,
                FilePath = a.FilePath,
                FileType = a.FileType,
                CreatedAt = a.CreatedAt
            }).ToList();

            return new PagedResponse<DisplayAttachmentDto>(
                dtos,
                filters.PageNumber,
                filters.PageSize,
                totalRecords
            );
        }

        private IQueryable<Attachment> ApplySorting(IQueryable<Attachment> query, string? sortColumn, string? sortOrder)
        {
            if (string.IsNullOrEmpty(sortColumn))
                sortColumn = "Id";
            
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "asc";

            sortColumn = sortColumn.ToLower();
            sortOrder = sortOrder.ToLower();

            return sortColumn switch
            {
                "id" => sortOrder == "desc" ? query.OrderByDescending(a => a.Id) : query.OrderBy(a => a.Id),
                "feedbackid" => sortOrder == "desc" ? query.OrderByDescending(a => a.FeedbackId) : query.OrderBy(a => a.FeedbackId),
                "filepath" => sortOrder == "desc" ? query.OrderByDescending(a => a.FilePath) : query.OrderBy(a => a.FilePath),
                "filetype" => sortOrder == "desc" ? query.OrderByDescending(a => a.FileType) : query.OrderBy(a => a.FileType),
                "createdat" => sortOrder == "desc" ? query.OrderByDescending(a => a.CreatedAt) : query.OrderBy(a => a.CreatedAt),
                _ => query.OrderBy(a => a.Id) 
            };
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

            attachment.FeedbackId = updateAttachmentDto.FeedbackId;
            attachment.FilePath = updateAttachmentDto.FilePath;
            attachment.FileType = updateAttachmentDto.FileType;
            attachment.CreatedAt = DateTime.SpecifyKind(attachment.CreatedAt, DateTimeKind.Utc);
            
            await _attachmentRepository.UpdateAsync(attachment);
        }

        public async Task DeleteAttachmentAsync(int id)
        {
            await _attachmentRepository.DeleteAsync(id);
        }
    }
}