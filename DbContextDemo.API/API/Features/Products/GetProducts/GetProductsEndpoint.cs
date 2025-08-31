using DbContextDemo.API.Domain;
using DbContextDemo.API.Infrastructure.Repositories.Interfaces;

namespace DbContextDemo.API.API.Features.Products.GetProducts;

public static class GetProductsEndpoint
{
    public static RouteGroupBuilder MapGetProductsEndpoint(this RouteGroupBuilder builder)
    {
        builder.MapGet("/", async (IGenericRepository<Product> repository) =>
            {
                var products = await repository.GetAllAsync();
                return Results.Ok(products);
            })
            .WithName("GetProducts")
            .Produces<IEnumerable<Product>>(StatusCodes.Status200OK)
            .WithOpenApi();
        return builder;
    }
}
