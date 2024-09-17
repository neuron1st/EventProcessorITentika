using Entities;

namespace Generator.Services;

public class EventGeneratorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Random _random = new Random();
    private const int IntervalSeconds = 2;

    public EventGeneratorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var waitTime = _random.Next(0, IntervalSeconds * 1000);
            await Task.Delay(waitTime, stoppingToken);

            var newEvent = GenerateEvent();

            await SendEventToProcessorAsync(newEvent);
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
        Console.WriteLine($"Send event: Id = {newEvent.Id}, Type = {newEvent.Type}, Time = {newEvent.Time}");

        await Task.Delay(500);
    }
}
