using Application.DTO;
using Application.DTO.Category;

namespace Application.Interfaces
{
    public interface ICategoryService
    {
        Task<PagedResponse<DisplayCategoryDto>> GetAllCategoriesAsync(FilterCategoryDto filters);
        Task<DisplayCategoryDto?> GetCategoryByIdAsync(int id);
        Task<DisplayCategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto); 
        Task UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto);
        Task DeleteCategoryAsync(int id);
    }
}