using DbContextDemo.Persistance;
using DbContextDemo.Persistance.Models;

namespace DbContextDemo.API.API.Endpoints.Orders.GET;

public static class GetOrderEndpoint
{
    public static RouteGroupBuilder MapGetOrderEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapGet("/{id:Guid}", async (Guid id, IGenericRepository<Order> repository) =>
            {
                var order = await repository.GetByIdAsync(id);
                return order is not null ? Results.Ok(order) : Results.NotFound();
            })
            .WithName("GetOrder")
            .Produces<Order>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi();
        return builder;
    }
}
