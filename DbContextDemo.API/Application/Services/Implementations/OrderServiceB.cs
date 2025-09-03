using DbContextDemo.API.API.Features.Orders.PostOrder;
using DbContextDemo.API.Application.Services.Interfaces;
using DbContextDemo.API.Domain;
using DbContextDemo.API.Infrastructure;
using DbContextDemo.API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DbContextDemo.API.Application.Services.Implementations;

/// <summary>
/// order service b does context passing with a transaction
/// </summary>
public sealed class OrderServiceB : IOrderService
{
    private readonly IUsesDbContextFactoryRepository<Order> orderRepository;
    private readonly IUsesDbContextFactoryRepository<Customer> customerRepository;
    private readonly IUsesDbContextFactoryRepository<Product> productRepository;
    private readonly IUsesDbContextFactoryRepository<Invoice> invoiceRepository;
    private readonly IUsesDbContextFactoryRepository<Shipment> shipmentRepository;
    private readonly ILogger<OrderServiceB> logger;
    private const decimal TAX = 0.06M;

    public OrderServiceB(
        IUsesDbContextFactoryRepository<Order> orderRepository,
        IUsesDbContextFactoryRepository<Customer> customerRepository,
        IUsesDbContextFactoryRepository<Product> productRepository,
        IUsesDbContextFactoryRepository<Invoice> invoiceRepository,
        IUsesDbContextFactoryRepository<Shipment> shipmentRepository,
        ILogger<OrderServiceB> logger)
    {
        this.orderRepository = orderRepository;
        this.customerRepository = customerRepository;
        this.productRepository = productRepository;
        this.invoiceRepository = invoiceRepository;
        this.shipmentRepository = shipmentRepository;
        this.logger = logger;

        var id = Guid.NewGuid();
        logger.LogInformation("New Order Service B {id}", id);
    }

    public async Task<PostOrderResponse> PlaceOrderAsync(PostOrderRequest req, CancellationToken ct = default)
    {
        await using var dbContext = await orderRepository.GetDbContextAsync().ConfigureAwait(false);
        await using var transaction = await dbContext.Database.BeginTransactionAsync(ct).ConfigureAwait(false);

        try
        {
            var customer = await dbContext.Set<Customer>().FindAsync(new object?[] { req.CustomerId }, ct).ConfigureAwait(false);
            if (customer is null) throw new InvalidOperationException("Customer does not exist");

            var productIds = req.OrderItems.Select(oi => oi.ProductId).ToList();
            var products = await dbContext.Set<Product>().Where(p => productIds.Contains(p.Id)).ToListAsync(ct).ConfigureAwait(false);
            if (products is null || !products.Any()) throw new InvalidOperationException("No Products were found");

            decimal total = 0.00M;
            foreach (var item in req.OrderItems)
            {
                var productToPurchase = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (productToPurchase is not null)
                {
                    total += productToPurchase.Price * item.Quantity;
                }
                else
                {
                    throw new InvalidOperationException($"No Product {item.ProductId} was found");
                }
            }

            total *= 1.00M + TAX;

            var newOrder = new Order
            {
                CustomerId = req.CustomerId,
                OrderDate = req.OrderDate,
                OrderItems = req.OrderItems.Select(x => new OrderItem { ProductId = x.ProductId, Quantity = x.Quantity }).ToList(),
            };

            await orderRepository.AddAsync(newOrder, dbContext).ConfigureAwait(false);

            await invoiceRepository.AddAsync(new Invoice
            {
                OrderId = newOrder.Id,
                CustomerId = customer.Id,
                Status = InvoiceStatuses.Unpaid.Status,
                Amount = total,
                InvoiceDt = DateTime.UtcNow
            }, dbContext).ConfigureAwait(false);

            await shipmentRepository.AddAsync(new Shipment
            {
                OrderId = newOrder.Id,
                AddressId = customer.AddressId,
                Status = ShipmentStatuses.PendingPayment.Status
            }, dbContext).ConfigureAwait(false);

            await transaction.CommitAsync(ct).ConfigureAwait(false);

            return new PostOrderResponse(newOrder.Id);
        }
        catch
        {
            await transaction.RollbackAsync(ct).ConfigureAwait(false);
            throw;
        }
    }
}

