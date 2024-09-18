using Context.Entities;

namespace Generator.Services;

public class EventGeneratorService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventGeneratorService> _logger;

    private readonly Random _random = new Random();
    private const int IntervalSeconds = 2;

    public EventGeneratorService(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory, ILogger<EventGeneratorService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var waitTime = _random.Next(0, IntervalSeconds * 1000);
            await Task.Delay(waitTime, stoppingToken);

            var newEvent = GenerateEvent();

            try
            {
                await SendEventToProcessorAsync(newEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending event to processor. Event Id: {EventId}", newEvent.Id);
            }
        }
    }

    public Event GenerateEvent()
    {
        var types = Enum.GetValues(typeof(EventTypeEnum));
        return new Event
        {
            Id = Guid.NewGuid(),
            Type = (EventTypeEnum)_random.Next(types.Length),
            Time = DateTime.UtcNow,
        };
    }

    public async Task SendEventToProcessorAsync(Event newEvent)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync("https://localhost:5001/api/Incident/events", newEvent);
            response.EnsureSuccessStatusCode();
            _logger.LogInformation("Sent event: Id = {EventId}, Type = {EventType}, Time = {EventTime}", newEvent.Id, newEvent.Type, newEvent.Time);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error sending request to processor. Event Id: {EventId}", newEvent.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while sending event. Event Id: {EventId}", newEvent.Id);
        }
    }
}
