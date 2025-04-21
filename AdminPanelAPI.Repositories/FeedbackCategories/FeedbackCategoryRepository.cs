using AdminPanelAPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminPanelAPI.Repositories.FeedbackCategories
{
    public class FeedbackCategoryRepository : IFeedbackCategoryRepository
    {
        private readonly AppDbContext _context;

        public FeedbackCategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FeedbackCategory>> GetAllAsync()
        {
            return await _context.FeedbackCategories.ToListAsync();
        }

        public async Task<FeedbackCategory?> GetByIdAsync(int id)
        {
            return await _context.FeedbackCategories.FindAsync(id);
        }

        public async Task AddAsync(FeedbackCategory feedbackCategory)
        {
            if (feedbackCategory is null)
            {
                throw new ArgumentNullException(nameof(feedbackCategory));
            }
            await _context.FeedbackCategories.AddAsync(feedbackCategory);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(FeedbackCategory feedbackCategory)
        {
            if (feedbackCategory is null)
            {
                throw new ArgumentNullException(nameof(feedbackCategory));
            }
            _context.Entry(feedbackCategory).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var feedbackCategory = await _context.FeedbackCategories.FindAsync(id);
            if (feedbackCategory != null)
            {
                _context.FeedbackCategories.Remove(feedbackCategory);
                await _context.SaveChangesAsync();
            }
        }
    }
}