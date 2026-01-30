using System.Text.Json;
using Shopy.Infrastructure.EventBus.Produce;
using Shopy.OrderService.Domain;

namespace Shopy.OrderService;

public class OrderApplicationService(IRabbitMQPublisher publisher)
{
    /// <summary>
    /// Create a new order and publish OrderCreatedEvent
    /// </summary>
    public async Task<Order> CreateOrderAsync(
        string customerId,
        string customerEmail,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var order = new Order(Guid.NewGuid(), customerId, customerEmail, amount);
        
        Console.WriteLine($"[Orders] Created order {order.Id:D} for customer {customerId}");
        
        // Publish all domain events
        await PublishEventsAsync(order, cancellationToken);
        
        return order;
    }
    
    /// <summary>
    /// Approve an order and publish OrderApprovedEvent
    /// </summary>
    public async Task<Order> ApproveOrderAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        order.Approve();
        
        Console.WriteLine($"[Orders] Approved order {order.Id:D}");
        
        // Publish all domain events
        await PublishEventsAsync(order, cancellationToken);
        
        return order;
    }
    
    /// <summary>
    /// Complete an order and publish OrderCompletedEvent
    /// </summary>
    public async Task<Order> CompleteOrderAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        order.Complete();
        
        Console.WriteLine($"[Orders] Completed order {order.Id:D}");
        
        // Publish all domain events
        await PublishEventsAsync(order, cancellationToken);
        
        return order;
    }
    
    /// <summary>
    /// Internal method to publish all domain events from the aggregate
    /// </summary>
    private async Task PublishEventsAsync(Order order, CancellationToken cancellationToken)
    {
        foreach (var @event in order.GetDomainEvents())
        {
            await publisher.PublishAsync(@event, cancellationToken);
        }
        
        order.ClearDomainEvents();
    }
}
