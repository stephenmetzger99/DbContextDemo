using DbContextDemo.API.Domain.Base;

namespace DbContextDemo.API.Domain;

public sealed class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set;  }

}
