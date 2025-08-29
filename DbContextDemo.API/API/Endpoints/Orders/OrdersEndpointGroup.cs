namespace DbContextDemo.API.API.Endpoints.Orders;

public static class OrdersEndpointGroup
{
    public static RouteGroupBuilder MapOrdersEndpointGroup(this RouteGroupBuilder builder)
    {
        builder.MapGroup("/order")
            .WithTags("Orders")
            .WithOpenApi();
        return builder;
    }
}

