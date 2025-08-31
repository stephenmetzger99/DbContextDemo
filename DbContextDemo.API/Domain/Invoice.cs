using DbContextDemo.API.Domain.Base;

namespace DbContextDemo.API.Domain;

public class Invoice : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; } = 0.00M;

    public DateTime InvoiceDt { get; set; }

    public string Status { get; set; } = InvoiceStatuses.Unpaid.Status;

}
public sealed record InvoiceStatuses(string Status)
{
    public static readonly InvoiceStatuses Unpaid = new("Unpaid");
    public static readonly InvoiceStatuses Paid = new("Paid");
    public static readonly InvoiceStatuses Cancelled = new("Cancelled");
 
}