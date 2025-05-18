using Microsoft.AspNetCore.Authorization;
using Services.Categories;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.Category;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DisplayCategoryDto>>> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories.ToList());
    }

    [HttpPost]
    public async Task<ActionResult<DisplayCategoryDto>> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetAllCategories), category);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
    {
        await _categoryService.UpdateCategoryAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}