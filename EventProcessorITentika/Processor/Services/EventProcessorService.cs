using Context;
using Context.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Processor.Services
{
    public class EventProcessorService : BackgroundService
    {
        private readonly IDbContextFactory<ProcessorDbContext> _contextFactory;

        private readonly Channel<Event> _eventChannel = Channel.CreateUnbounded<Event>();
        private readonly ConcurrentQueue<(Event, DateTime)> _type2Events = new ConcurrentQueue<(Event, DateTime)>();
        private readonly TimeSpan _compositeTemplateTimeLimit = TimeSpan.FromSeconds(20);

        public EventProcessorService(IDbContextFactory<ProcessorDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach(var evt in _eventChannel.Reader.ReadAllAsync(stoppingToken))
            {
                if (evt.Type == EventTypeEnum.Type1)
                {
                    await ProcessType1Event(evt);
                }
                else if (evt.Type == EventTypeEnum.Type2)
                {
                    AddType2Event(evt);
                }
            }
        }

        public void EnqueueEvent(Event evt)
        {
            _eventChannel.Writer.TryWrite(evt);
        }

        private async Task ProcessType1Event(Event evt)
        {
            CleanExpiredType2Events();

            if (_type2Events.TryPeek(out var type2Event) && (DateTime.UtcNow - type2Event.Item2 < _compositeTemplateTimeLimit))
            {
                await CreateIncident(IncidentTypeEnum.Type2, evt, type2Event.Item1);
                _type2Events.TryDequeue(out _);
            }
            else
            {
                await CreateIncident(IncidentTypeEnum.Type1, evt);
            }
        }

        private void AddType2Event(Event evt)
        {
            _type2Events.Enqueue((evt, DateTime.UtcNow));
        }

        private void CleanExpiredType2Events()
        {
            while (_type2Events.TryPeek(out var oldestEvent) && (DateTime.UtcNow - oldestEvent.Item2) > _compositeTemplateTimeLimit)
            {
                _type2Events.TryDequeue(out _);
            }
        }

        private async Task CreateIncident(IncidentTypeEnum type, params Event[] events)
        {
            var incident = new Incident
            {
                Id = Guid.NewGuid(),
                Type = type,
                Time = DateTime.UtcNow,
                Events = events.ToList()
            };

            using var context = await _contextFactory.CreateDbContextAsync();

            context.Incidents.Add(incident);
            await context.SaveChangesAsync();

            //Console.WriteLine($"Created incident: Id = {incident.Id}, Type = {incident.Type}, Time = {incident.Time}");
        }

        public async Task<List<Incident>> GetIncidents(int offset, int limit)
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            var incidents = await context
                .Incidents
                .Include(i => i.Events)
                .Skip(Math.Max(offset, 0))
                .Take(Math.Max(0, Math.Min(limit, 1000)))
                .ToListAsync();

            return incidents;
        }
    }
}
