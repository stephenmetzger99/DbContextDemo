using DbContextDemo.API.API.Features.Addresses.GetAddress;
using DbContextDemo.API.API.Features.Addresses.GetAddresses;

namespace DbContextDemo.API.API.Features.Addresses;

public static class AddressesEndpointGroup
{
    public static RouteGroupBuilder MapAddressEndpointGroup(this RouteGroupBuilder builder)
    {
        var addresses = builder.MapGroup("/addresses")
            .WithTags("Addresses")
            .WithOpenApi();

        addresses.MapGetAddressesEndpoint();
        addresses.MapGetAddressEndpoint();

        return builder;
    }
}
