using System.Collections.Generic;
using System.Threading.Tasks;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

namespace Services.FeedbackCategories
{
    public interface IFeedbackCategoryService
    {
        Task AddCategoryToFeedbackAsync(CreateFeedbackCategoryDto dto);
        Task RemoveCategoryFromFeedbackAsync(int feedbackId, int categoryId);
        Task<List<DisplayCategoryDto>> GetCategoriesByFeedbackAsync(int feedbackId);
        
        // Остальные методы
        Task<IEnumerable<DisplayFeedbackCategoryDto>> GetAllFeedbackCategoriesAsync();
        Task<DisplayFeedbackCategoryDto?> GetFeedbackCategoryByIdAsync(int id);
        Task UpdateFeedbackCategoryAsync(UpdateFeedbackCategoryDto updateFeedbackCategoryDto);
        Task DeleteFeedbackCategoryAsync(int id);
    }
}