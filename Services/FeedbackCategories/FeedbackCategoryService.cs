using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Repositories.FeedbackCategories;
using Data.Models;
using Repositories.Categories;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.DTOs.Update;
using Services.DTOs.Display; // Import DisplayCategoryDto

namespace Services.FeedbackCategories
{
    public class FeedbackCategoryService : IFeedbackCategoryService
    {
        private readonly IFeedbackCategoryRepository _feedbackCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;

        public FeedbackCategoryService(
            IFeedbackCategoryRepository feedbackCategoryRepository,
            ICategoryRepository categoryRepository) // Corrected the type here
        {
            _feedbackCategoryRepository = feedbackCategoryRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task AddCategoryToFeedbackAsync(CreateFeedbackCategoryDto dto)
        {
            var feedbackCategory = new FeedbackCategory
            {
                FeedbackId = dto.FeedbackId,
                CategoryId = dto.CategoryId
            };
            await _feedbackCategoryRepository.AddAsync(feedbackCategory);
        }

        public async Task RemoveCategoryFromFeedbackAsync(int feedbackId, int categoryId)
        {
            // Implementation depends on how your repository is set up. This is a possible solution.
            var feedbackCategory = await _feedbackCategoryRepository.GetAllAsync();
            var feedbackCategoryToDelete = ((IEnumerable<object>)feedbackCategory).Cast<FeedbackCategory>().FirstOrDefault(fc => fc.FeedbackId == feedbackId && fc.CategoryId == categoryId);

            if (feedbackCategoryToDelete != null)
            {
                await _feedbackCategoryRepository.DeleteAsync(feedbackCategoryToDelete.Id);
            }
        }

        public async Task<List<DisplayCategoryDto>> GetCategoriesByFeedbackAsync(int feedbackId)
        {
            var feedbackCategories = ((IEnumerable<object>)await _feedbackCategoryRepository.GetAllAsync()).Cast<FeedbackCategory>().Where(fc => fc.FeedbackId == feedbackId).ToList();

            var categoryIds = feedbackCategories.Select(fc => fc.CategoryId).ToList();
            
            var categories = new List<Category>();
            foreach (var categoryId in categoryIds)
            {
                var category = await _categoryRepository.GetByIdAsync(categoryId);
                if (category != null)
                {
                    categories.Add(category);
                }
            }
            //  Map to DisplayCategoryDto and return
            return categories.Select(c => new DisplayCategoryDto
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                Description = c.Description
            }).ToList();
        }

        public async Task<IEnumerable<DisplayFeedbackCategoryDto>> GetAllFeedbackCategoriesAsync()
        {
            var feedbackCategories = await _feedbackCategoryRepository.GetAllAsync();
            return feedbackCategories.Select(fc => new DisplayFeedbackCategoryDto
            {
                Id = fc.Id,
                FeedbackId = fc.FeedbackId,
                CategoryId = fc.CategoryId
            });
        }

        public async Task<DisplayFeedbackCategoryDto?> GetFeedbackCategoryByIdAsync(int id)
        {
            var feedbackCategory = await _feedbackCategoryRepository.GetByIdAsync(id);
            return feedbackCategory == null ? null : new DisplayFeedbackCategoryDto
            {
                Id = feedbackCategory.Id,
                FeedbackId = feedbackCategory.FeedbackId,
                CategoryId = feedbackCategory.CategoryId
            };
        }

        public async Task UpdateFeedbackCategoryAsync(UpdateFeedbackCategoryDto updateFeedbackCategoryDto)
        {
            var feedbackCategory = await _feedbackCategoryRepository.GetByIdAsync(updateFeedbackCategoryDto.Id);
            if (feedbackCategory == null)
            {
                throw new ArgumentException($"FeedbackCategory with id {updateFeedbackCategoryDto.Id} not found.");
            }

            feedbackCategory.FeedbackId = updateFeedbackCategoryDto.FeedbackId.Value;
            feedbackCategory.CategoryId = updateFeedbackCategoryDto.CategoryId.Value;

            await _feedbackCategoryRepository.UpdateAsync(feedbackCategory);
        }

        public async Task DeleteFeedbackCategoryAsync(int id)
        {
            await _feedbackCategoryRepository.DeleteAsync(id);
        }
    }
}