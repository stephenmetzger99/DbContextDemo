using DbContextDemo.API.Domain;
using DbContextDemo.API.Infrastructure.Repositories.Interfaces;

namespace DbContextDemo.API.API.Features.Addresses.GetAddresses;

public static class GetAddressesEndpoint
{
    public static RouteGroupBuilder MapGetAddressesEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (IGenericRepository<Address> repository) =>
        {
            var addresses = await repository.GetAllAsync();
            return Results.Ok(addresses);
        })
        .WithName("GetAllAddresses")
        .Produces<List<Address>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status500InternalServerError);
        return group;
    }
}
