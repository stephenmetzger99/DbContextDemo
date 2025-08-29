namespace DbContextDemo.API.API.Endpoints.Payments;

public static class InvoicesEndpointGroup
{
    public static RouteGroupBuilder MapInvoicesEndpointGroup(this RouteGroupBuilder builder)
    {
        var payments = builder.MapGroup("/invoices")
            .WithTags("Invoices")
            .WithOpenApi();
        return builder;
    }
}
