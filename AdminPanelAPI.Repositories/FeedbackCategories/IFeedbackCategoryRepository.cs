using AdminPanelAPI.Data.Models;

namespace AdminPanelAPI.Repositories.FeedbackCategories
{
    public interface IFeedbackCategoryRepository
    {
        Task<IEnumerable<FeedbackCategory>> GetAllAsync();
        Task<FeedbackCategory?> GetByIdAsync(int id);
        Task AddAsync(FeedbackCategory feedbackCategory);
        Task UpdateAsync(FeedbackCategory feedbackCategory);
        Task DeleteAsync(int id);
    }
}