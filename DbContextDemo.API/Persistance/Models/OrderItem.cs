using DbContextDemo.API.Persistance.Models.Base;

namespace DbContextDemo.Persistance.Models;

public sealed class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set;  }
}
