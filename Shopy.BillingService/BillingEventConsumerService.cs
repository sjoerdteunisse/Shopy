using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shopy.Core.Events;
using Shopy.Infrastructure.EventBus.Consume;

namespace Shopy.BillingService;

public class BillingEventConsumerService(
    IRabbitMQConsumer consumer,
    IServiceProvider serviceProvider)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[Billing] Event consumer service starting...");
        Console.ResetColor();
        
        if (consumer is RabbitMQConsumer rabbitConsumer)
        {
            await rabbitConsumer.InitializeAsync(stoppingToken);
        }
        
        await consumer.SubscribeAsync(
            serviceName: "BillingService",
            queueName: "billing-service",
            handler: HandleEventAsync,
            cancellationToken: stoppingToken);
    }
    
    private async Task HandleEventAsync(IDomainEvent @event, CancellationToken cancellationToken)
    {
        var eventType = @event.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
        var handlers = serviceProvider.GetServices(handlerType);

        var handlerCollection = handlers as object?[] ?? handlers.ToArray();
        
        if (handlerCollection.Length == 0)
        {
            Console.WriteLine($"No handlers for {@event.EventType}");
            Console.ResetColor();
            return;
        }
        
        var handleMethod = handlerType.GetMethod("HandleAsync",
            [eventType, typeof(CancellationToken)]);
        
        var tasks = handlerCollection
            .Select(handler => (Task)handleMethod!.Invoke(handler,
                [@event, cancellationToken])!)
            .ToList();
        
        await Task.WhenAll(tasks);
    }
}