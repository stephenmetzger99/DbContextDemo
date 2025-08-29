using DbContextDemo.API.Persistance.Models.Base;

namespace DbContextDemo.Persistance.Models;

public sealed class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; } = 0.00M;
}
