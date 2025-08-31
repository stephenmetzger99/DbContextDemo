using DbContextDemo.API.Domain;
using DbContextDemo.API.Persistance.Repositories.Interfaces;

namespace DbContextDemo.API.API.Features.Orders.GetOrder;

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
