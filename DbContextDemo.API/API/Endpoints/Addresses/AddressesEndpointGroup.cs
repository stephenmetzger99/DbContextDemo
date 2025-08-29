namespace DbContextDemo.API.API.Endpoints.Addresses;

public static class AddressesEndpointGroup
{
    public static RouteGroupBuilder MapAddressEndpointGroup(this RouteGroupBuilder builder)
    {
        builder.MapGroup("/addresses")
            .WithTags("Addresses")
            .WithOpenApi();
        return builder;
    }
}
