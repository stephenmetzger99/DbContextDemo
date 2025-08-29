namespace DbContextDemo.API.API.Endpoints.Payments;

public static class PaymentsEndpointGroup
{
    public static RouteGroupBuilder MapPaymentsEndpointGroup(this RouteGroupBuilder builder)
    {
        builder.MapGroup("/payments")
            .WithTags("Payments")
            .WithOpenApi();
        return builder;
    }
}
