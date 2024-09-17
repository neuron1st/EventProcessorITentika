using Entities;
using Generator.Services;
using Microsoft.AspNetCore.Mvc;

namespace Generator.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventController : ControllerBase
{
    private readonly EventGeneratorService _eventGeneratorService;

    public EventController(EventGeneratorService eventGeneratorService)
    {
        _eventGeneratorService = eventGeneratorService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateManualEvent([FromBody] EventRequest request)
    {
        var newEvent = new Event
        {
            Id = Guid.NewGuid(),
            Type = request.Type,
            Time = DateTime.Now,
        };
        await _eventGeneratorService.SendEventToProcessorAsync(newEvent);
        return Ok(newEvent);
    }
}
