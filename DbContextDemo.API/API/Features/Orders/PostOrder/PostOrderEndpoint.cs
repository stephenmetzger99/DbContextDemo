using DbContextDemo.API.API.Features.Orders.PostOrder;
using DbContextDemo.API.API.Services;
using DbContextDemo.Persistance;

namespace DbContextDemo.API.API.Endpoints.Orders.POST;

public static class PostOrderEndpoint
{
    public static RouteGroupBuilder MapPostOrderEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapPost("/", async (PostOrderRequest order, IOrderService service) =>
            {
                var orderId = await service.PlaceOrderAsync(order);
                return Results.Created($"/orders/{orderId}", order);
            })
            .WithName("PostOrder")
            .Produces<Guid>(StatusCodes.Status201Created)
            .WithOpenApi();
        return builder;
    }
}
