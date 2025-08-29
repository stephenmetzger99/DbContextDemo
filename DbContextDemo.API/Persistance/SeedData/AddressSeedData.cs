namespace DbContextDemo.API.Persistance.SeedData;

using DbContextDemo.Persistance.Models;
using System.Collections.Generic;

public static class AddressSeedData
{
    public static List<Address> GetSeedAddresses() => new()
    {
        new() { Street = "100 Main St", City = "Springfield", State = "IL", ZipCode = 62701 },
        new() { Street = "101 Main St", City = "Springfield", State = "IL", ZipCode = 62702 },
        new() { Street = "102 Main St", City = "Springfield", State = "IL", ZipCode = 62703 },
        new() { Street = "103 Main St", City = "Springfield", State = "IL", ZipCode = 62704 },
        new() { Street = "104 Main St", City = "Springfield", State = "IL", ZipCode = 62705 },
        new() { Street = "105 Main St", City = "Springfield", State = "IL", ZipCode = 62706 },
        new() { Street = "106 Main St", City = "Springfield", State = "IL", ZipCode = 62707 },
        new() { Street = "107 Main St", City = "Springfield", State = "IL", ZipCode = 62708 },
        new() { Street = "108 Main St", City = "Springfield", State = "IL", ZipCode = 62709 },
        new() { Street = "109 Main St", City = "Springfield", State = "IL", ZipCode = 62710 }
    };
}