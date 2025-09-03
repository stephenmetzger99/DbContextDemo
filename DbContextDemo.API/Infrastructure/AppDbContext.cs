using DbContextDemo.API.Domain.Base;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DbContextDemo.API.Infrastructure;

public class AppDbContext : DbContext
{

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        Debug.WriteLine($"AppDbContext constructed: {ContextId.InstanceId}");

    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        Debug.WriteLine($"AppDbContext save changes async: {ContextId.InstanceId}");

        return await base.SaveChangesAsync(ct);
    }

    public override async ValueTask DisposeAsync()
    {
        Debug.WriteLine($"AppDbContext disposing async: {ContextId.InstanceId}");
        await base.DisposeAsync();
    }

    public override void Dispose()
    {
        Debug.WriteLine($"AppDbContext disposing sync: {ContextId.InstanceId}");

        base.Dispose();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>();
        modelBuilder.Entity<OrderItem>();
        modelBuilder.Entity<Product>();
        modelBuilder.Entity<Customer>(b =>
        {
            b.HasOne<Address>()                      
             .WithMany()                             
             .HasForeignKey(c => c.AddressId)        
             .IsRequired()
             .OnDelete(DeleteBehavior.Restrict);     
            b.HasIndex(c => c.AddressId);            
        });
        modelBuilder.Entity<Invoice>(b =>
        {
            b.HasOne<Order>()
            .WithMany()
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(o => o.OrderId);

            b.HasOne<Customer>()
           .WithMany()
           .HasForeignKey(s => s.CustomerId)
           .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(o => o.CustomerId);
        });

        modelBuilder.Entity<Shipment>(b =>
        {
            b.HasOne<Order>()
            .WithMany()
            .HasForeignKey(s => s.OrderId)
            .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(o => o.OrderId);

            b.HasOne<Address>()
           .WithMany()
           .HasForeignKey(s => s.AddressId)
           .OnDelete(DeleteBehavior.NoAction);

            b.HasIndex(o => o.AddressId);
        });
        modelBuilder.Entity<Address>();

        // apply to every entity that inherits BaseEntity
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.Id))
                    .ValueGeneratedNever();
            }
        }



        base.OnModelCreating(modelBuilder);

    }

}