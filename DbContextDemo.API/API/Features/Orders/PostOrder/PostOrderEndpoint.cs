using DbContextDemo.API.Application.Services.Interfaces;

namespace DbContextDemo.API.API.Features.Orders.PostOrder;

public static class PostOrderEndpoint
{
    public static RouteGroupBuilder MapPostOrderEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapPost("/", async (PostOrderRequest order, IOrderService service) =>
            {
                var orderResponse = await service.PlaceOrderAsync(order);
                return Results.Ok(orderResponse);
            })
            .WithName("PostOrder")
            .Produces<PostOrderResponse>(StatusCodes.Status200OK)
            .WithOpenApi();
        return builder;
    }
}
