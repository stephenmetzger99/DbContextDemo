using DbContextDemo.API.Persistance.Models.Base;

namespace DbContextDemo.Persistance.Models;

public sealed class Shipment : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid AddressId { get; set; } = new();
    public string Status { get; set; } = ShipmentStatuses.PendingPayment.Status;

}

public sealed record ShipmentStatuses(string Status)
{
    public static readonly ShipmentStatuses PendingPayment = new("PendingPayment");
    public static readonly ShipmentStatuses InProcess = new("InProcess");
    public static readonly ShipmentStatuses Shipped = new("Shipped");
    public static readonly ShipmentStatuses Delivered = new("Delivered");
    public static readonly ShipmentStatuses Cancelled = new("Cancelled");
}
