namespace Shopy.Core.Events.Orders;

public class OrderCreatedEvent(Guid orderId, string customerId, string customerEmail, decimal amount)
    : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    
    public string EventType
    {
        get => nameof(OrderCreatedEvent);
        init { }
    }

    public Guid OrderId { get; init; } = orderId;
    public string CustomerId { get; init; } = customerId;
    public string CustomerEmail { get; init; } = customerEmail;
    public decimal Amount { get; init; } = amount;

}
