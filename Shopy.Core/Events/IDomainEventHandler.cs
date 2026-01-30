namespace Shopy.Core.Events;

public interface IDomainEventHandler<in T> where T : IDomainEvent
{
    Task HandleAsync(T @event, CancellationToken cancellationToken = default);
}
