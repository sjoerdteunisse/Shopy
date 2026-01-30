using Shopy.Infrastructure.EventBus;
using Shopy.Infrastructure.EventBus.Produce;
using Shopy.OrderService;

var config = new RabbitMQConfiguration();
var logger = new ConsoleEventBusLogger("OrderService");

var publisherFactory = new RabbitMQPublisherFactory(config, logger, "OrderService");
var publisher = await publisherFactory.CreateAsync();

try
{
    var orderService = new OrderApplicationService(publisher);

    var order1 = await orderService.CreateOrderAsync(
        "CUST-001",
        "john@example.com",
        150.50m
    );
    await Task.Delay(1000);

    await orderService.ApproveOrderAsync(order1);
    await Task.Delay(1000);

    await orderService.CompleteOrderAsync(order1);

    Console.WriteLine("\n[OrderService] All events published");
}
catch(SystemException ex)
{
}

 