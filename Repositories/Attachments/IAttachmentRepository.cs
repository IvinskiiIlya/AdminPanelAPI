using Data.Models;

namespace Repositories.Attachments
{
    public interface IAttachmentRepository
    {
        Task<IEnumerable<Attachment>> GetAllAsync();
        Task<Attachment?> GetByIdAsync(int id);
        Task<IEnumerable<Attachment>> GetByFeedbackIdAsync(int feedbackId);
        Task AddAsync(Attachment attachment);
        Task UpdateAsync(Attachment attachment);
        Task DeleteAsync(int id);
    }
}