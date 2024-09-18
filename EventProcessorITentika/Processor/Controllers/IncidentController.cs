using Context.Entities;
using Microsoft.AspNetCore.Mvc;
using Processor.Services;

namespace Processor.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncidentController : ControllerBase
{
    private readonly EventProcessorService _eventProcessorService;

    public IncidentController(EventProcessorService eventProcessorService)
    {
        _eventProcessorService = eventProcessorService;
    }

    [HttpPost("events")]
    public async Task<IActionResult> ReceiveEvent([FromBody] Event evt)
    {
        _eventProcessorService.EnqueueEvent(evt);
        return Ok();
    }

    [HttpGet("incidents")]
    public async Task<IActionResult> GetIncidents([FromQuery] int offset = 0, [FromQuery] int limit = 10)
    {
        var incidents = await _eventProcessorService.GetIncidents(offset, limit);

        return Ok(incidents);
    }
}
