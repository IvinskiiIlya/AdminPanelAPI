using Services.DTO.Category;

namespace Services.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<DisplayCategoryDto>> GetAllCategoriesAsync();
        Task<DisplayCategoryDto?> GetCategoryByIdAsync(int id);
        Task<DisplayCategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto); 
        Task UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
        Task DeleteCategoryAsync(int id);
    }
}