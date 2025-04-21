using AdminPanelAPI.Data.Models;

namespace AdminPanelAPI.Repositories.Attachments
{
    public interface IAttachmentRepository
    {
        Task<IEnumerable<Attachment>> GetAllAsync();
        Task<Attachment?> GetByIdAsync(int id);
        Task AddAsync(Attachment attachment);
        Task UpdateAsync(Attachment attachment);
        Task DeleteAsync(int id);
    }
}