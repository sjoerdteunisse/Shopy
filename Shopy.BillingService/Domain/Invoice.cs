namespace Shopy.BillingService.Domain;

public class Invoice(Guid orderId, string customerId, decimal amount)
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; } = orderId;
    public string CustomerId { get; set; } = customerId;
    public decimal Amount { get; set; } = amount;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public void FinalizeOrder()
    {
        Status = InvoiceStatus.Finalized;
    }
    
    public void MarkAsPaid()
    {
        Status = InvoiceStatus.Paid;
    }
}

public enum InvoiceStatus
{
    Draft = 0,
    Finalized = 1,
    Paid = 2,
    Overdue = 3
}
