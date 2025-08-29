using DbContextDemo.API.Persistance.Models.Base;

namespace DbContextDemo.Persistance.Models;

public sealed class Order : BaseEntity
{
    public Guid CustomerId { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public DateTime OrderDate { get; set; }
}
