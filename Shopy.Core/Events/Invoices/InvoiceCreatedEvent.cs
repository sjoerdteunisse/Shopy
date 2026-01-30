namespace Shopy.Core.Events.Invoices;

public class InvoiceCreatedEvent(Guid invoiceId, Guid orderId, string customerId, decimal invoiceAmount) : IDomainEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();
    public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    public string EventType
    {
        get => nameof(InvoiceCreatedEvent);
        init { }
    }

    public Guid InvoiceId { get; } = invoiceId;
    public Guid OrderId { get; } = orderId;
    public string CustomerId { get; } = customerId;
    public decimal InvoiceAmount { get; } = invoiceAmount;
}