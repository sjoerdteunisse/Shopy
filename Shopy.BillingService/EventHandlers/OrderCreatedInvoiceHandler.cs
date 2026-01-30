using Shopy.BillingService.Domain;
using Shopy.Core.Events;
using Shopy.Core.Events.Orders;

namespace Shopy.BillingService.EventHandlers;

public class OrderCreatedInvoiceHandler : IDomainEventHandler<OrderCreatedEvent>
{
    public async Task HandleAsync(OrderCreatedEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"\n[Billing] OrderCreatedInvoiceHandler processing event...");
        Console.WriteLine($"Event ID: {@event.EventId:D}");
        Console.WriteLine($"Order ID: {@event.OrderId:D}");
        Console.WriteLine($"Customer: {@event.CustomerId}");
        Console.WriteLine($"Amount: ${@event.Amount:F2}");
        
        // Create invoice in Billing service domain
        var invoice = new Invoice(@event.OrderId, @event.CustomerId, @event.Amount);
        
        Console.WriteLine($" Created invoice {invoice.Id:D}");
        
        // Simulate saving to billing database
        await Task.Delay(300, cancellationToken);
        
        Console.WriteLine($"Invoice saved to billing database");
    }
}