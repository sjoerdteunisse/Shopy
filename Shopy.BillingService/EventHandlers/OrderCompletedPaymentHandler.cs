using Shopy.Core.Events;
using Shopy.Core.Events.Orders;

namespace Shopy.BillingService.EventHandlers;

public class OrderCompletedPaymentHandler : IDomainEventHandler<OrderCompletedEvent>
{
    public async Task HandleAsync(OrderCompletedEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"\n[Billing] OrderCompletedPaymentHandler processing event...");
        Console.WriteLine($"Event ID: {@event.EventId:D}");
        Console.WriteLine($"Order ID: {@event.OrderId:D}");
        Console.WriteLine($"Payment amount: ${@event.Amount:F2}");
        
        // Process payment in Billing service
        Console.WriteLine($"Processing payment...");
        await Task.Delay(500, cancellationToken);
        
        Console.WriteLine($"Updating accounts receivable");
        await Task.Delay(200, cancellationToken);
        
        Console.WriteLine($"Recording revenue");
        await Task.Delay(200, cancellationToken);
        
        Console.WriteLine($"Payment processed and recorded");
    }
}