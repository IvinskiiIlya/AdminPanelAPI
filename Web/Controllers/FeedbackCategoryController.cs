using Services;
using Services.DTOs.Create;
using Services.DTOs.Display;
using Services.FeedbackCategories;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/feedback-categories")] // Четкий URL, а не стандартный [controller]
public class FeedbackCategoryController : ControllerBase
{
    private readonly IFeedbackCategoryService _feedbackCategoryService;

    public FeedbackCategoryController(IFeedbackCategoryService feedbackCategoryService)
    {
        _feedbackCategoryService = feedbackCategoryService;
    }

    // Добавление категории к Feedback
    [HttpPost]
    public async Task<ActionResult> AddCategoryToFeedback([FromBody] CreateFeedbackCategoryDto dto)
    {
        await _feedbackCategoryService.AddCategoryToFeedbackAsync(dto);
        return NoContent();
    }

    // Удаление категории из Feedback
    [HttpDelete("{feedbackId}/{categoryId}")]
    public async Task<ActionResult> RemoveCategoryFromFeedback(int feedbackId, int categoryId)
    {
        await _feedbackCategoryService.RemoveCategoryFromFeedbackAsync(feedbackId, categoryId);
        return NoContent();
    }

    // Получение всех категорий Feedback
    [HttpGet("{feedbackId}")]
    public async Task<ActionResult<List<DisplayCategoryDto>>> GetCategoriesByFeedback(int feedbackId)
    {
        var categories = await _feedbackCategoryService.GetCategoriesByFeedbackAsync(feedbackId);
        return Ok(categories);
    }
}