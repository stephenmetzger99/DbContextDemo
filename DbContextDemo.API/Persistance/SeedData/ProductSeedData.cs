namespace DbContextDemo.API.Persistance.SeedData;

using DbContextDemo.API.Domain;
using System.Collections.Generic;


public static class ProductSeedData
{
    public static List<Product> GetSeedProducts()
    {
        var products = new List<Product>();
        var random = new Random();
        for (int i = 1; i <= 1000; i++)
        {
            products.Add(new Product
            {
                Name = $"Product {i}",
                Description = $"Description for product {i}",
                Price = random.Next(10, 1001) + 0.99M
            });
        }
        return products;
    }
}