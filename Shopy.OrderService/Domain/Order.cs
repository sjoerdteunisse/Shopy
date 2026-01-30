using Shopy.Core.Events;
using Shopy.Core.Events.Orders;

namespace Shopy.OrderService.Domain;

public class Order
{
    public Guid Id { get; private set; }
    public string CustomerId { get; private set; }
    public string CustomerEmail { get; private set; }
    public decimal Amount { get; private set; }
    public OrderStatus Status { get; private set; }
    
    private readonly List<IDomainEvent> _domainEvents = new();
    
    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents.AsReadOnly();
    public void ClearDomainEvents() => _domainEvents.Clear();
    
    public Order(Guid id, string customerId, string customerEmail, decimal amount)
    {
        Id = id;
        CustomerId = customerId;
        CustomerEmail = customerEmail;
        Amount = amount;
        Status = OrderStatus.Created;
        
        _domainEvents.Add(new OrderCreatedEvent(id, customerId, customerEmail, amount));
    }
    
    public void Approve()
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException($"Cannot approve order in {Status} status");
        
        Status = OrderStatus.Approved;
        _domainEvents.Add(new OrderApprovedEvent(Id, CustomerId, Amount, DateTime.UtcNow));
    }
    
    public void Complete()
    {
        if (Status != OrderStatus.Approved)
            throw new InvalidOperationException($"Cannot complete order in {Status} status");
        
        Status = OrderStatus.Completed;
        _domainEvents.Add(new OrderCompletedEvent(Id, CustomerId, Amount));
    }
}

public enum OrderStatus
{
    Created = 1,
    Approved = 2,
    Completed = 3,
    Failed = 4
}