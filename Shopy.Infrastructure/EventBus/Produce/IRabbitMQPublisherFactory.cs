namespace Shopy.Infrastructure.EventBus.Produce;

/// <summary>
/// Factory to create publisher asynchronously
/// </summary>
public interface IRabbitMQPublisherFactory
{
    Task<IRabbitMQPublisher> CreateAsync(CancellationToken cancellationToken = default);
}