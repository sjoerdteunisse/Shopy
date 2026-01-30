namespace Shopy.Core.Events;

public interface IDomainEvent
{
    public Guid EventId { get; init; }
    public DateTime OccurredAt { get; init; }
    public string EventType { get; init; }
}