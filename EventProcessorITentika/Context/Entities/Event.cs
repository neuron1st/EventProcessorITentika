namespace Context.Entities;

public class Event
{
    public Guid Id { get; set; }
    public EventTypeEnum Type { get; set; }
    public DateTime Time { get; set; }
}

public enum EventTypeEnum
{
    Type1,
    Type2,
    Type3,
    Type4
}
