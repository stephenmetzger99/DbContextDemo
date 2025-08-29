using DbContextDemo.Persistance;

namespace DbContextDemo.API.API.Endpoints.Products.GET;

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
