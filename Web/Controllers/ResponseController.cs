using Microsoft.AspNetCore.Authorization;
using Services.Responses;
using Microsoft.AspNetCore.Mvc;
using Services.DTO.Response;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResponseController : ControllerBase
{
    
    private readonly IResponseService _responseService;

    public ResponseController(IResponseService responseService)
    {
        _responseService = responseService;
    }

    [HttpGet("{feedbackId}")]
    public async Task<ActionResult<List<DisplayResponseDto>>> GetResponsesByFeedback(int feedbackId)
    {
        var responses = await _responseService.GetResponsesByFeedbackAsync(feedbackId);
        return Ok(responses);
    }

    [HttpPost]
    public async Task<ActionResult<DisplayResponseDto>> CreateResponse([FromBody] CreateResponseDto dto)
    {
        var response = await _responseService.CreateResponseAsync(dto);
        return CreatedAtAction(
            nameof(GetResponsesByFeedback), 
            new { feedbackId = dto.FeedbackId }, 
            response
        );
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateResponse(int id, [FromBody] UpdateResponseDto dto)
    {
        await _responseService.UpdateResponseAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteResponse(int id)
    {
        await _responseService.DeleteResponseAsync(id);
        return NoContent();
    }
}