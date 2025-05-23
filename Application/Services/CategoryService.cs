using Application.DTO;
using Application.DTO.Category;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;

namespace Application.Services
{
    public class CategoryService : ICategoryService
    {
        
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<PagedResponse<DisplayCategoryDto>> GetAllCategoriesAsync(FilterCategoryDto filters)
        {
            var categories = await _categoryRepository.GetAllAsync();
            var filtered = categories.AsQueryable();

            if (!string.IsNullOrEmpty(filters.SearchTerm))
                filtered = filtered.Where(c => c.Name.Contains(filters.SearchTerm));
            else if (!string.IsNullOrEmpty(filters.Name))
            {
                filtered = filtered.Where(c => c.Name.Contains(filters.Name));
            }

            var totalRecords = filtered.Count();

            var paged = filtered
                .OrderBy(c => c.Id)
                .Skip((filters.PageNumber - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            var dtos = paged.Select(c => new DisplayCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            }).ToList();

            return new PagedResponse<DisplayCategoryDto>(
                dtos,
                filters.PageNumber,
                filters.PageSize,
                totalRecords
            );
        }
        
        public async Task<DisplayCategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;
            
            return new DisplayCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task<DisplayCategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                Name = createCategoryDto.Name,
                Description = createCategoryDto.Description
            };
            
            await _categoryRepository.AddAsync(category);
            
            return new DisplayCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                throw new ArgumentException($"Категория с id = {id} не найдена.");

            category.Name = updateCategoryDto.Name!;
            category.Description = updateCategoryDto.Description;

            await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _categoryRepository.DeleteAsync(id);
        }
    }
}