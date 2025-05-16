using Data;
using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Attachments
{
    public class AttachmentRepository : IAttachmentRepository
    {
        
        private readonly AppDbContext _context;

        public AttachmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Attachment>> GetAllAsync()
        {
            return await _context.Attachments.ToListAsync();
        }

        public async Task<Attachment?> GetByIdAsync(int id)
        {
            return await _context.Attachments.FindAsync(id);
        }
        
        public async Task<IEnumerable<Attachment>> GetByFeedbackIdAsync(int feedbackId)
        {
            return await _context.Attachments.Where(a => a.FeedbackId == feedbackId).ToListAsync();
        }

        public async Task AddAsync(Attachment attachment)
        {
            if (attachment is null)
                throw new ArgumentNullException(nameof(attachment));
            await _context.Attachments.AddAsync(attachment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Attachment attachment)
        {
            if (attachment is null)
                throw new ArgumentNullException(nameof(attachment));
            _context.Entry(attachment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var attachment = await _context.Attachments.FindAsync(id);
            if (attachment != null)
            {
                _context.Attachments.Remove(attachment);
                await _context.SaveChangesAsync();
            }
        }
    }
}