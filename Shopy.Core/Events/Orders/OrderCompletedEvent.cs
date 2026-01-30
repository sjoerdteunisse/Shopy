using System.Text.Json.Serialization;

namespace Shopy.Core.Events.Orders;

public class OrderCompletedEvent(Guid orderId, string customerId, decimal amount) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    
    public string EventType
    {
        get => nameof(OrderCompletedEvent);
        init { }
    }

    public Guid OrderId { get; init; } = orderId;
    public string CustomerId { get; init; } = customerId;
    public decimal Amount { get; init; } = amount;
}
