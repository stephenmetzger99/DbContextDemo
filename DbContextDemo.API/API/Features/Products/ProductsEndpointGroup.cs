using DbContextDemo.API.API.Endpoints.Products.GET;

namespace DbContextDemo.API.API.Endpoints.Products;

public static class ProductsEndpointGroup
{
    public static RouteGroupBuilder MapProductsEndpointGroup(this RouteGroupBuilder builder)
    {
        var products = builder.MapGroup("/products")
            .WithTags("Products")
            .WithOpenApi();

        products.MapGetProductsEndpoint();

        return builder;
    }
}
