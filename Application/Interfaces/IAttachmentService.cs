using Application.DTO.Attachment;

namespace Application.Interfaces
{
    public interface IAttachmentService
    {
        Task<IEnumerable<DisplayAttachmentDto>> GetAllAttachmentsAsync();
        Task<List<DisplayAttachmentDto>> GetAttachmentsByFeedbackAsync(int feedbackId);
        Task<DisplayAttachmentDto?> GetAttachmentByIdAsync(int id);
        Task<DisplayAttachmentDto> CreateAttachmentAsync(CreateAttachmentDto createAttachmentDto);
        Task UpdateAttachmentAsync(UpdateAttachmentDto updateAttachmentDto);
        Task DeleteAttachmentAsync(int id);
    }
}