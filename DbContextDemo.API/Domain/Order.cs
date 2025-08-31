using DbContextDemo.API.Domain.Base;

namespace DbContextDemo.API.Domain;

public sealed class Order : BaseEntity
{
    public Guid CustomerId { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public DateTime OrderDate { get; set; }
}
