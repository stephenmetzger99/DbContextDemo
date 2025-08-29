namespace DbContextDemo.API.API.Endpoints.Shipments;

public static class ShipmentsEndpointGroup
{
    public static RouteGroupBuilder MapShipmentsEndpointGroup(this RouteGroupBuilder builder)
    {
        var shipments  = builder.MapGroup("/shipments")
            .WithTags("Shipments")
            .WithOpenApi();
        return builder;
    }
}
