using Application.DTO;
using Application.DTO.Attachment;

namespace Application.Interfaces
{
    public interface IAttachmentService
    {
        Task<PagedResponse<DisplayAttachmentDto>> GetAllAttachmentsAsync(FilterAttachmentDto filters);
        Task<DisplayAttachmentDto?> GetAttachmentByIdAsync(int id);
        Task<List<DisplayAttachmentDto>> GetAttachmentsByFeedbackAsync(int feedbackId);
        Task<DisplayAttachmentDto> CreateAttachmentAsync(CreateAttachmentDto createAttachmentDto);
        Task UpdateAttachmentAsync(UpdateAttachmentDto updateAttachmentDto);
        Task DeleteAttachmentAsync(int id);
    }
}