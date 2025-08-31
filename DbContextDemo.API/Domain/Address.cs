using DbContextDemo.API.Domain.Base;

namespace DbContextDemo.API.Domain;

public sealed class Address : BaseEntity
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public int ZipCode { get; set; }



}
