using Shopy.Core.Events;
using Shopy.Core.Events.Orders;

namespace Shopy.BillingService.EventHandlers;

public class OrderApprovedNotificationHandler : IDomainEventHandler<OrderApprovedEvent>
{
    public async Task HandleAsync(OrderApprovedEvent @event, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"\n[Billing] OrderApprovedNotificationHandler processing event...");
        Console.WriteLine($"Order ID: {@event.OrderId:D}");
        Console.WriteLine($"Customer: {@event.CustomerId}");
        Console.WriteLine($"Amount: ${@event.Amount:F2}");
        
        Console.WriteLine($"Updating customer billing account");
        await Task.Delay(300, cancellationToken);
        
        Console.WriteLine($"Billing account updated");
    }
}