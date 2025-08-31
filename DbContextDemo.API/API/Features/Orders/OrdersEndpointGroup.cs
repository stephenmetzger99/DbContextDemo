using DbContextDemo.API.API.Features.Orders.GetOrder;
using DbContextDemo.API.API.Features.Orders.PostOrder;

namespace DbContextDemo.API.API.Features.Orders;

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

