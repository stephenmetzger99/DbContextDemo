using DbContextDemo.API.API.Endpoints.Addresses;
using DbContextDemo.API.API.Endpoints.Customers;
using DbContextDemo.API.API.Endpoints.Orders;
using DbContextDemo.API.API.Endpoints.Payments;
using DbContextDemo.API.API.Endpoints.Products;
using DbContextDemo.API.API.Endpoints.Shipments;
using DbContextDemo.API.API.Services;
using DbContextDemo.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Generic repo resolves to your concrete repo
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IUsesDbContextFactoryRepository<>), typeof(UsesDbContextFactoryRepository<>));

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
    await DbContextDemo.Persistance.SeedData.DbSeeder.SeedAsync(db);
#endif
}


app.UseHttpsRedirection();

var api = app.MapGroup("/api/v1/");

api.MapProductsEndpointGroup();
api.MapOrdersEndpointGroup();
api.MapAddressEndpointGroup();
api.MapShipmentsEndpointGroup();
api.MapCustomersEndpointGroup();
api.MapInvoicesEndpointGroup();

app.Run();

