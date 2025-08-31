using DbContextDemo.API.API.Features.Products.GetProducts;

namespace DbContextDemo.API.API.Features.Products;

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
