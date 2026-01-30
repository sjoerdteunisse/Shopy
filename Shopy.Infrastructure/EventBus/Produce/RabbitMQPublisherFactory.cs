using Shopy.Infrastructure.EventBus.Produce;

namespace Shopy.Infrastructure.EventBus;

public class RabbitMQPublisherFactory(
    RabbitMQConfiguration config,
    IEventBusLogger logger,
    string serviceName)
    : IRabbitMQPublisherFactory
{
    public async Task<IRabbitMQPublisher> CreateAsync(CancellationToken cancellationToken = default)
    {
        var publisher = new RabbitMQPublisher(config, logger, serviceName);
        await publisher.InitializeAsync(cancellationToken);
        return publisher;
    }
    
}