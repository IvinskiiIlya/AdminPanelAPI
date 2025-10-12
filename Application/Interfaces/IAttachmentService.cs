using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTO;
using Application.DTO.Attachment;

namespace Application.Interfaces
{
    public interface IAttachmentService
    {
        Task<PagedResponse<DisplayAttachmentDto>> GetAllAttachmentsAsync(FilterAttachmentDto filters);
        Task<DisplayAttachmentDto?> GetAttachmentByIdAsync(int id);
        Task<List<DisplayAttachmentDto>> GetAttachmentsByFeedbackAsync(int feedbackId);
        Task<List<DisplayAttachmentDto>> GetAttachmentsByUserIdAsync(string userId);
        Task<DisplayAttachmentDto> CreateAttachmentAsync(CreateAttachmentDto createAttachmentDto);
        Task UpdateAttachmentAsync(UpdateAttachmentDto updateAttachmentDto);
        Task DeleteAttachmentAsync(int id);
    }
}