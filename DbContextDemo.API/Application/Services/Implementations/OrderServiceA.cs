using DbContextDemo.API.API.Features.Orders.PostOrder;
using DbContextDemo.API.Application.Services.Interfaces;
using DbContextDemo.API.Domain;
using DbContextDemo.API.Infrastructure;
using DbContextDemo.API.Infrastructure.Repositories.Interfaces;
using System.Linq;

namespace DbContextDemo.API.Application.Services.Implementations;

/// <summary>
/// <see cref="OrderServiceA"/> uses the <see cref="IUsesDbContextFactoryRepository{T}"/> but without an explicit transaction.
/// Each repository method creates a new <see cref="AppDbContext"/> via the factory.
/// </summary>
public sealed class OrderServiceA : IOrderService
{
    private readonly IUsesDbContextFactoryRepository<Order> orderRepository;
    private readonly IUsesDbContextFactoryRepository<Customer> customerRepository;
    private readonly IUsesDbContextFactoryRepository<Product> productRepository;
    private readonly IUsesDbContextFactoryRepository<Invoice> invoiceRepository;
    private readonly IUsesDbContextFactoryRepository<Shipment> shipmentRepository;
    private readonly ILogger<OrderServiceA> logger;
    private const decimal TAX = 0.06M;

    public OrderServiceA(
        IUsesDbContextFactoryRepository<Order> orderRepository,
        IUsesDbContextFactoryRepository<Customer> customerRepository,
        IUsesDbContextFactoryRepository<Product> productRepository,
        IUsesDbContextFactoryRepository<Invoice> invoiceRepository,
        IUsesDbContextFactoryRepository<Shipment> shipmentRepository,
        ILogger<OrderServiceA> logger)
    {
        this.orderRepository = orderRepository;
        this.customerRepository = customerRepository;
        this.productRepository = productRepository;
        this.invoiceRepository = invoiceRepository;
        this.shipmentRepository = shipmentRepository;
        this.logger = logger;

        var id = Guid.NewGuid();
        logger.LogInformation("New Order Service A {id}", id);
    }

    public async Task<PostOrderResponse> PlaceOrderAsync(PostOrderRequest req, CancellationToken ct = default)
    {
        var customer = await customerRepository.GetByIdAsync(req.CustomerId).ConfigureAwait(false);
        if (customer is null) throw new InvalidOperationException("Customer does not exist");

        var productIds = req.OrderItems.Select(oi => oi.ProductId);
        var products = (await productRepository.GetByIdsAsync(productIds).ConfigureAwait(false)).ToList();
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

        await orderRepository.AddAsync(newOrder).ConfigureAwait(false);

        await invoiceRepository.AddAsync(new Invoice
        {
            OrderId = newOrder.Id,
            CustomerId = customer.Id,
            Status = InvoiceStatuses.Unpaid.Status,
            Amount = total,
            InvoiceDt = DateTime.UtcNow
        }).ConfigureAwait(false);

        await shipmentRepository.AddAsync(new Shipment
        {
            OrderId = newOrder.Id,
            AddressId = customer.AddressId,
            Status = ShipmentStatuses.PendingPayment.Status
        }).ConfigureAwait(false);

        return new PostOrderResponse(newOrder.Id);
    }
}

