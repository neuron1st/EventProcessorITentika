using Context.Entities;

namespace Generator.Services;

public class EventGeneratorService : BackgroundService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly Random _random = new Random();
    private const int IntervalSeconds = 2;

    public EventGeneratorService(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _serviceProvider = serviceProvider;
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
                Console.WriteLine("Error sending event to processor");
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
            //Console.WriteLine($"Send event: Id = {newEvent.Id}, Type = {newEvent.Type}, Time = {newEvent.Time}");

        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine("Error sending request to processor");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unexpected error occurred");
        }
    }
}
