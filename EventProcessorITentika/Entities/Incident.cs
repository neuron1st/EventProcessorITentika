namespace Entities;

public class Incident
{
    public Guid Id { get; set; }
    public IncidentTypeEnum Type { get; set; }
    public DateTime Time { get; set; }
}

public enum IncidentTypeEnum
{
    Type1,
    Type2
}
