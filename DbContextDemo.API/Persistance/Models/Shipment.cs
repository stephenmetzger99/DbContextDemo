using DbContextDemo.API.Persistance.Models.Base;

namespace DbContextDemo.Persistance.Models;

public sealed class Shipment : BaseEntity
{
    public int OrderId { get; set; }
    public int AddressId { get; set; } = new();
    public string Status { get; set; } = "Pending";

}
