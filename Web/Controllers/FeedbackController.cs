using Services.Feedbacks;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.Feedback;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    
    private readonly IFeedbackService _feedbackService;

    public FeedbackController(IFeedbackService feedbackService)
    {
        _feedbackService = feedbackService;
    }

    [HttpGet]
    public async Task<ActionResult<List<DisplayFeedbackDto>>> GetAllFeedbacks()
    {
        var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
        return Ok(feedbacks.ToList());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DisplayFeedbackDto>> GetFeedbackById(int id)
    {
        var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
        return feedback == null ? NotFound() : Ok(feedback);
    }

    [HttpPost]
    public async Task<ActionResult<DisplayFeedbackDto>> CreateFeedback([FromBody] CreateFeedbackDto dto)
    {
        var feedback = await _feedbackService.CreateFeedbackAsync(dto);
        return CreatedAtAction(nameof(GetFeedbackById), new { id = feedback.Id }, feedback);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateFeedback(int id, [FromBody] UpdateFeedbackDto dto)
    {
        await _feedbackService.UpdateFeedbackAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteFeedback(int id)
    {
        await _feedbackService.DeleteFeedbackAsync(id);
        return NoContent();
    }
}