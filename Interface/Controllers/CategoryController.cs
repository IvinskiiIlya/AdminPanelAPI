using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTO.Category;
using Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace Interface.Controllers;

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

    /// <summary>
    /// Получить список всех категорий
    /// </summary>
    /// <returns>Список категорий</returns>
    [HttpGet]
    [SwaggerOperation(
        Summary = "Получить все категории",
        Description = "Возвращает полный список категорий."
    )]
    [SwaggerResponse(200, "Список категорий успешно получен", typeof(List<DisplayCategoryDto>))]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<List<DisplayCategoryDto>>> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        return Ok(categories.ToList());
    }

    /// <summary>
    /// Создать новую категорию
    /// </summary>
    /// <param name="dto">Данные для создания категории</param>
    /// <returns>Созданная категория</returns>
    [HttpPost]
    [SwaggerOperation(
        Summary = "Создать категорию",
        Description = "Создает новую категорию с указанными данными."
    )]
    [SwaggerResponse(201, "Категория успешно создана", typeof(DisplayCategoryDto))]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    public async Task<ActionResult<DisplayCategoryDto>> CreateCategory([FromBody] CreateCategoryDto dto)
    {
        var category = await _categoryService.CreateCategoryAsync(dto);
        return CreatedAtAction(nameof(GetAllCategories), category);
    }

    /// <summary>
    /// Обновить существующую категорию
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <param name="dto">Данные для обновления категории</param>
    [HttpPut("{id}")]
    [SwaggerOperation(
        Summary = "Обновить категорию",
        Description = "Обновляет данные категории по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Категория успешно обновлена")]
    [SwaggerResponse(400, "Некорректные данные запроса")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Категория не найдена")]
    public async Task<ActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
    {
        await _categoryService.UpdateCategoryAsync(id, dto);
        return NoContent();
    }

    /// <summary>
    /// Удалить категорию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    [HttpDelete("{id}")]
    [SwaggerOperation(
        Summary = "Удалить категорию",
        Description = "Удаляет категорию по указанному идентификатору."
    )]
    [SwaggerResponse(204, "Категория успешно удалена")]
    [SwaggerResponse(401, "Пользователь не авторизован")]
    [SwaggerResponse(404, "Категория не найдена")]
    public async Task<ActionResult> DeleteCategory(int id)
    {
        await _categoryService.DeleteCategoryAsync(id);
        return NoContent();
    }
}
