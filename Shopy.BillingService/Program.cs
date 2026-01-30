using Shopy.Core.Events;
using Shopy.BillingService;
using Shopy.BillingService.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shopy.Infrastructure.EventBus;
using Shopy.Infrastructure.EventBus.Consume;
using Shopy.Infrastructure.EventBus.Produce;

var config = new RabbitMQConfiguration();
var logger = new ConsoleEventBusLogger("BillingService");

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices(services =>
{
    services.AddSingleton(config);
    services.AddSingleton<IEventBusLogger>(logger);
    
    services.AddSingleton<IRabbitMQConsumer>(sp => new RabbitMQConsumer(config, logger));
    
    RegisterEventHandlers(services);
    
    services.AddHostedService<BillingEventConsumerService>();
});

var host = builder.Build();

await host.RunAsync();

static void RegisterEventHandlers(IServiceCollection services)
{
    var assembly = typeof(OrderCreatedInvoiceHandler).Assembly;
    var handlerType = typeof(IDomainEventHandler<>);
    
    var handlers = assembly.GetTypes()
        .Where(t => t is { IsClass:  true, IsAbstract: false })
        .Where(t => t.GetInterfaces().Any(i =>
            i.IsGenericType && i.GetGenericTypeDefinition() == handlerType))
        .ToList();
    
    foreach (var handler in handlers)
    {
        var interfaces = 
            handler
            .GetInterfaces()
            .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == handlerType);
        
        foreach (var @interface in interfaces)
        {
            services.AddScoped(@interface, handler);
            Console.WriteLine($"Registered: {handler.Name}");
        }
    }
}