using DbContextDemo.API.Domain;
using DbContextDemo.API.Infrastructure.SeedData;
using Microsoft.EntityFrameworkCore;

namespace DbContextDemo.API.Infrastructure;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context, CancellationToken ct = default)
    {
        // Ensure DB is up to date first (optional if you do this in Program.cs)
        // await context.Database.MigrateAsync(ct);

        using var tx = await context.Database.BeginTransactionAsync(ct);

        if (!await context.Set<Address>().AnyAsync(ct))
        {
            await context.Set<Address>().AddRangeAsync(AddressSeedData.GetSeedAddresses(), ct);
            await context.SaveChangesAsync(ct); 
        }

        if (!await context.Set<Customer>().AnyAsync(ct))
        {
            IList<Address> addresses = await context.Set<Address>().ToListAsync();

            foreach (var cust in CustomerSeedData.GetSeedCustomers())
            {
                var random = new Random();
                ;
                var randomAddressId = addresses[random.Next(addresses.Count)].Id;
                cust.AddressId = randomAddressId;
                await context.Set<Customer>().AddAsync(cust, ct);

            }
        }

        if (!await context.Set<Product>().AnyAsync(ct))
        {
            await context.Set<Product>().AddRangeAsync(ProductSeedData.GetSeedProducts(), ct);
        }

        await context.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }
}
