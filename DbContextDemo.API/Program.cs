using DbContextDemo.API.API.Features.Addresses;
using DbContextDemo.API.API.Features.Customers;
using DbContextDemo.API.API.Features.Invoices;
using DbContextDemo.API.API.Features.Orders;
using DbContextDemo.API.API.Features.Products;
using DbContextDemo.API.API.Features.Shipments;
using DbContextDemo.API.API.Services;
using DbContextDemo.API.Persistance;
using DbContextDemo.API.Persistance.Repositories.Implementations;
using DbContextDemo.API.Persistance.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    .LogTo(Console.WriteLine,
           new[] { DbLoggerCategory.Database.Transaction.Name },
           LogLevel.Information),
    contextLifetime: ServiceLifetime.Scoped,
    optionsLifetime: ServiceLifetime.Singleton);

builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




// Generic repo resolves to your concrete repo
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUsesDbContextFactoryRepository<>), typeof(UsesDbContextFactoryRepository<>));
builder.Services.AddScoped(typeof(IUsesAmbientDbContextRepository<>), typeof(UsesAmbientDbContextRepository<>));

builder.Services.AddScoped<IOrderService, OrderService>();


builder.Services.AddEndpointsApiExplorer();   // <-- add this
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});



var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); 
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    });
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();               // make sure schema exists
#if DEBUG
    await DbSeeder.SeedAsync(db);
#endif
}


app.UseHttpsRedirection();

var api = app.MapGroup("/api/v1/");

api.MapProductsEndpointGroup();
api.MapOrdersEndpointGroup();
api.MapAddressEndpointGroup();
api.MapCustomersEndpointGroup();
api.MapShipmentsEndpointGroup();
api.MapInvoicesEndpointGroup();

app.Run();

