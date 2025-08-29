namespace DbContextDemo.API.API.Endpoints.Products;

public static class ProductsEndpointGroup
{
    public static RouteGroupBuilder MapProductsEndpointGroup(this RouteGroupBuilder builder)
    {
        builder.MapGroup("/products")
            .WithTags("Products")
            .WithOpenApi();
        return builder;
    }
}
