using DbContextDemo.API.API.Endpoints.Orders.GET;
using DbContextDemo.API.API.Endpoints.Orders.POST;

namespace DbContextDemo.API.API.Endpoints.Orders;

public static class OrdersEndpointGroup
{
    public static RouteGroupBuilder MapOrdersEndpointGroup(this RouteGroupBuilder builder)
    {
        var orders = builder.MapGroup("/orders")
            .WithTags("Orders")
            .WithOpenApi();

        orders.MapGetOrderEndpoint();
        orders.MapPostOrderEndpoint();

        return builder;
    }
}

