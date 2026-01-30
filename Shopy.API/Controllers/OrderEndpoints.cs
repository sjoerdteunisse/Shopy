using Microsoft.AspNetCore.Mvc;
using Shopy.API.Models;
using Shopy.OrderService;
using Shopy.OrderService.Domain;

namespace Shopy.API.Controllers;

public static class OrderEndpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        var orders = app.MapGroup("api/v1/orders")
            .WithTags("Orders")
            .WithOpenApi();

        // POST /api/orders - Create new order
        orders.MapPost("/", CreateOrderAsync)
            .WithName("CreateOrder")
            .WithSummary("Create a new order")
            .Produces<Order>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest);

        return app;
    }

    private static async Task<IResult> CreateOrderAsync(
        [FromBody] OrderRequest request,
        [FromServices] OrderApplicationService orderService,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerId))
            return Results.BadRequest("CustomerId is required");
        
        if (string.IsNullOrWhiteSpace(request.CustomerEmail))
            return Results.BadRequest("CustomerEmail is required");
        
        if (request.Amount <= 0)
            return Results.BadRequest("Amount must be greater than 0");

        var order = await orderService.CreateOrderAsync(
            request.CustomerId,
            request.CustomerEmail,
            request.Amount,
            cancellationToken);

        return Results.Ok(order);
    }
}