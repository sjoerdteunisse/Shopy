using Shopy.Core.Events;

namespace Shopy.Infrastructure.EventBus.Consume;

public interface IRabbitMQConsumer
{
    Task SubscribeAsync(
        string serviceName,
        string queueName,
        Func<IDomainEvent, CancellationToken, Task> handler,
        CancellationToken cancellationToken = default);
}