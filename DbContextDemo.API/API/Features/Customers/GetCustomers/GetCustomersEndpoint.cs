using DbContextDemo.API.Persistance.Repositories.Implementations;

namespace DbContextDemo.API.API.Features.Customers.GetCustomers;

public static class GetCustomersEndpoint
{
    public static RouteGroupBuilder MapGetCustomersEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IGenericRepository<Customer> repository) =>
        {
            var Customers = await repository.GetAllAsync();
            return Results.Ok(Customers);
        })
        .WithName("GetAllCustomers")
        .Produces<List<Customer>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);
        return group;
    }
}
