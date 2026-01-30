using Shopy.Core.Events;

namespace Shopy.Infrastructure.EventBus.Produce;

public interface IRabbitMQPublisher
{
    Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IDomainEvent;
}