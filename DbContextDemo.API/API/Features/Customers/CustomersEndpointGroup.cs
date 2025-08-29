using DbContextDemo.API.API.Features.Customers.GetCustomers;

namespace DbContextDemo.API.API.Endpoints.Customers;

public static class CustomersEndpointGroup
{
    public static RouteGroupBuilder MapCustomersEndpointGroup(this RouteGroupBuilder builder)
    {
        var customers = builder.MapGroup("/customers")
            .WithTags("Customers")
            .WithOpenApi();

        customers.MapGetCustomersEndpoint();
        return builder;
    }
}

