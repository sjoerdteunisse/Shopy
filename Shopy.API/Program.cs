using Shopy.OrderService;
using Shopy.API.Controllers;
using Shopy.Infrastructure.EventBus;
using Shopy.Infrastructure.EventBus.Produce;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var config = new RabbitMQConfiguration();
var logger = new ConsoleEventBusLogger("APIService");

builder.Services.AddSingleton(new RabbitMQPublisherFactory(config, logger, "OrderService").CreateAsync());
builder.Services.AddScoped<OrderApplicationService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapOrderEndpoints();
app.Run();