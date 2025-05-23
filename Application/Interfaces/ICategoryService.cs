using Application.DTO.Category;

namespace Application.Interfaces
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