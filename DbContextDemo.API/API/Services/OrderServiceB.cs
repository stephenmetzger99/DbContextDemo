using DbContextDemo.API.API.Features.Orders.PostOrder;
using DbContextDemo.API.Persistance.Repositories.Implementations;
using DbContextDemo.Persistance;
using Microsoft.EntityFrameworkCore;

namespace DbContextDemo.API.API.Services;

public sealed class OrderServiceB : IOrderService
{
    private readonly IUsesAmbientDbContextRepository<Order> orderRepository;
    private readonly IUsesAmbientDbContextRepository<Customer> customerRepository;
    private readonly IUsesAmbientDbContextRepository<Product> productRepository;
    private readonly IUsesAmbientDbContextRepository<Invoice> invoiceRepository;
    private readonly IUsesAmbientDbContextRepository<Shipment> shipmentRepository;
    private readonly IDbContextFactory<AppDbContext> dbFactory;
    private readonly ILogger<OrderServiceB> logger;
    private const decimal TAX = 0.06M;

    public OrderServiceB(
        IUsesAmbientDbContextRepository<Order> orderRepository,
        IUsesAmbientDbContextRepository<Customer> customerRepository,
        IUsesAmbientDbContextRepository<Product> productRepository,
        IUsesAmbientDbContextRepository<Invoice> invoiceRepository,
        IUsesAmbientDbContextRepository<Shipment> shipmentRepository,
        IDbContextFactory<AppDbContext> dbFactory,
        ILogger<OrderServiceB> logger)
    {
        this.orderRepository = orderRepository;
        this.customerRepository = customerRepository;
        this.productRepository = productRepository;
        this.invoiceRepository = invoiceRepository;
        this.shipmentRepository = shipmentRepository;
        this.dbFactory = dbFactory;
        this.logger = logger;

        var id = Guid.NewGuid();
        logger.LogInformation("New Order Service B {id}", id);
    }

    public async Task<PostOrderResponse> PlaceOrderAsync(PostOrderRequest req, CancellationToken ct = default)
    {
        PostOrderResponse response = default!;
        await dbFactory.InUowAsync(async () =>
        {
            var customer = await customerRepository.GetByIdAsync(req.CustomerId, ct).ConfigureAwait(false);
            if (customer is null) throw new InvalidOperationException("Customer does not exist");

            var productIds = req.OrderItems.Select(oi => oi.ProductId);
            var products = await productRepository.GetByIdsAsync(productIds, ct: ct).ConfigureAwait(false);
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

            total *= (1.00M + TAX);

            var newOrder = new Order
            {
                CustomerId = req.CustomerId,
                OrderDate = req.OrderDate,
                OrderItems = req.OrderItems.Select(x => new OrderItem { ProductId = x.ProductId, Quantity = x.Quantity }).ToList(),
            };

            await orderRepository.AddAsync(newOrder, ct).ConfigureAwait(false);

            await invoiceRepository.AddAsync(new Invoice
            {
                OrderId = newOrder.Id,
                CustomerId = customer.Id,
                Status = InvoiceStatuses.Unpaid.Status,
                Amount = total,
                InvoiceDt = DateTime.UtcNow
            }, ct).ConfigureAwait(false);

            await shipmentRepository.AddAsync(new Shipment
            {
                OrderId = newOrder.Id,
                AddressId = customer.AddressId,
                Status = ShipmentStatuses.PendingPayment.Status
            }, ct).ConfigureAwait(false);

            response = new PostOrderResponse(newOrder.Id);
        }, ct).ConfigureAwait(false);

        return response;
    }
}
