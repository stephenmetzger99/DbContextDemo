using DbContextDemo.API.Domain.Base;

namespace DbContextDemo.API.Domain;

public sealed class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid AddressId { get; set; }

}
