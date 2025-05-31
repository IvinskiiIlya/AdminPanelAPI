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
            var query = categories.AsQueryable();

            if (!string.IsNullOrEmpty(filters.Name))
            {
                var nameFilter = filters.Name.ToLower();
                query = query.Where(c => c.Name != null && c.Name.ToLower().Contains(nameFilter));
            }

            if (!string.IsNullOrEmpty(filters.Description))
            {
                var descFilter = filters.Description.ToLower();
                query = query.Where(c => c.Description != null && c.Description.ToLower().Contains(descFilter));
            }

            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var search = filters.SearchTerm.ToLower();
                query = query.Where(c =>
                    (!string.IsNullOrEmpty(c.Name) && c.Name.ToLower().Contains(search)) ||
                    (!string.IsNullOrEmpty(c.Description) && c.Description.ToLower().Contains(search))
                );
            }

            var totalRecords = query.Count();

            query = ApplySorting(query, filters.SortColumn, filters.SortOrder);

            var paged = query
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

        private IQueryable<Category> ApplySorting(IQueryable<Category> query, string? sortColumn, string? sortOrder)
        {
            if (string.IsNullOrEmpty(sortColumn))
                sortColumn = "Id";
            
            if (string.IsNullOrEmpty(sortOrder))
                sortOrder = "asc";

            sortColumn = sortColumn.ToLower();
            sortOrder = sortOrder.ToLower();

            return sortColumn switch
            {
                "id" => sortOrder == "desc" ? query.OrderByDescending(c => c.Id) : query.OrderBy(c => c.Id),
                "name" => sortOrder == "desc" ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
                "description" => sortOrder == "desc" ? query.OrderByDescending(c => c.Description) : query.OrderBy(c => c.Description),
                _ => query.OrderBy(c => c.Id)
            };
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
                Id = createCategoryDto.Id,
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