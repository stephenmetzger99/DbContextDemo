using DbContextDemo.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore.Design;
using DbContextDemo.API.API.Endpoints.Products;
using DbContextDemo.API.API.Endpoints.Orders;
using DbContextDemo.API.API.Endpoints.Addresses;
using DbContextDemo.API.API.Endpoints.Shipments;
using DbContextDemo.API.API.Endpoints.Customers;
using DbContextDemo.API.API.Endpoints.Payments;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Generic repo resolves to your concrete repo
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();               // make sure schema exists
#if DEBUG
    await DbContextDemo.Persistance.SeedData.DbSeeder.SeedAsync(db);
#endif
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var api = app.MapGroup("/api/v1/");

api.MapProductsEndpointGroup();
api.MapOrdersEndpointGroup();
api.MapAddressEndpointGroup();
api.MapShipmentsEndpointGroup();
api.MapCustomersEndpointGroup();
api.MapPaymentsEndpointGroup();

app.Run();

