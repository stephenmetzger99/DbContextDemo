using DbContextDemo.API.Persistance.Repositories.Implementations;
using DbContextDemo.Persistance.Models;

namespace DbContextDemo.API.API.Endpoints.Addresses.GET;

public static class GetAddressEndpoint
{
    public static RouteGroupBuilder MapGetAddressEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:Guid}", async (IGenericRepository<Address> repository, Guid id) =>
        {
            var address = await repository.GetByIdAsync(id);
            return address is not null ? Results.Ok(address) : Results.NotFound();
        })
        .WithName("GetAddress")
        .Produces<List<Address>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
        return group;
    }
}
