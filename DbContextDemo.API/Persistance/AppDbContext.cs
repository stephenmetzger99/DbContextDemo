using DbContextDemo.Persistance.Models;
using Microsoft.EntityFrameworkCore;

namespace DbContextDemo.Persistance;

public class AppDbContext : DbContext
{
    private readonly ILogger<AppDbContext> _logger;

    public AppDbContext(DbContextOptions<AppDbContext> options, ILogger<AppDbContext> logger)
        : base(options)
    {
        _logger = logger;
        _logger.LogInformation("AppDbContext constructed: {Id}", ContextId.InstanceId);
    }

    public override async ValueTask DisposeAsync()
    {
        _logger.LogInformation("AppDbContext disposing async: {Id}", ContextId.InstanceId);
        await base.DisposeAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>();
        modelBuilder.Entity<OrderItem>();
        modelBuilder.Entity<Product>();
        modelBuilder.Entity<Customer>();
        modelBuilder.Entity<Payment>();
        modelBuilder.Entity<Shipment>();
        modelBuilder.Entity<Address>();
    }
}