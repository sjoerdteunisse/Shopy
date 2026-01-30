namespace Shopy.Core.Events.Orders;

public class OrderApprovedEvent(Guid orderId, string customerId, decimal amount, DateTime approvedAt) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    
    public string EventType
    {
        get => nameof(OrderApprovedEvent);
        init { }
    }

    public Guid OrderId { get; init; } = orderId;
    public string CustomerId { get; init; } = customerId;
    public decimal Amount { get; init; } = amount;

    public DateTime ApprovedAt { get; set;}
}