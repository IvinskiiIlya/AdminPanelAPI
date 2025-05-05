using System.Collections.Generic;
using System.Threading.Tasks;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

namespace Services.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<DisplayCategoryDto>> GetAllCategoriesAsync();
        Task<DisplayCategoryDto?> GetCategoryByIdAsync(int id);
        Task<DisplayCategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto); // Изменено возвращаемое значение
        Task UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto); // Добавлен параметр id
        Task DeleteCategoryAsync(int id);
    }
}