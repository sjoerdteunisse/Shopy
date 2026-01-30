using Shopy.API.Controllers;
using Shopy.Infrastructure.EventBus;
using Shopy.Infrastructure.EventBus.Produce;
using Shopy.OrderService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var config = new RabbitMQConfiguration();
var logger = new ConsoleEventBusLogger("APIService");

var publisherFactory = new RabbitMQPublisherFactory(config, logger, "OrderService");
var publisher = await publisherFactory.CreateAsync();

builder.Services.AddSingleton<IRabbitMQPublisher>(publisher);
builder.Services.AddScoped<OrderApplicationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapOrderEndpoints();
app.Run();