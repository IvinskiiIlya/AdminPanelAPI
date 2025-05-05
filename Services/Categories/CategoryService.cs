using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.Categories;
using Data.Models;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;

namespace Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<DisplayCategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(c => new DisplayCategoryDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description
            });
        }

        public async Task<DisplayCategoryDto?> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;
            
            return new DisplayCategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task<DisplayCategoryDto> CreateCategoryAsync(CreateCategoryDto createCategoryDto)
        {
            var category = new Category
            {
                CategoryName = createCategoryDto.CategoryName,
                Description = createCategoryDto.Description
            };
            
            await _categoryRepository.AddAsync(category);
            
            return new DisplayCategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task UpdateCategoryAsync(int id, UpdateCategoryDto updateCategoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                throw new ArgumentException($"Category with id {id} not found.");
            }

            category.CategoryName = updateCategoryDto.CategoryName!;
            category.Description = updateCategoryDto.Description;

            await _categoryRepository.UpdateAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _categoryRepository.DeleteAsync(id);
        }
    }
}