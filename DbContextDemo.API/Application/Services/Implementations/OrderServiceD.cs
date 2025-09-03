using DbContextDemo.API.API.Features.Orders.PostOrder;
using DbContextDemo.API.Application.Services.Interfaces;
using DbContextDemo.API.Domain;
using DbContextDemo.API.Infrastructure.Repositories.Interfaces;

namespace DbContextDemo.API.Application.Services.Implementations;
/// <summary>
/// <see cref="OrderServiceD"/> uses plain injection of scoped DbContexts
/// </summary>
public sealed class OrderServiceD : IOrderService
{
    private readonly IGenericRepository<Order> orderRepository;
    private readonly IGenericRepository<Customer> customerRepository;
    private readonly IGenericRepository<Product> productRepository;
    private readonly IGenericRepository<Invoice> invoiceRepository;
    private readonly IGenericRepository<Shipment> shipmentRepository;
    private readonly ILogger<OrderServiceD> logger;

    public OrderServiceD(
        IGenericRepository<Order> orderRepository,
        IGenericRepository<Customer> customerRepository,
        IGenericRepository<Product> productRepository,
        IGenericRepository<Invoice> invoiceRepository,
        IGenericRepository<Shipment> shipmentRepository,
        ILogger<OrderServiceD> logger)
    {
        this.orderRepository = orderRepository;
        this.customerRepository = customerRepository;
        this.productRepository = productRepository;
        this.invoiceRepository = invoiceRepository;
        this.shipmentRepository = shipmentRepository;
        this.logger = logger;

        var id = Guid.NewGuid();
        logger.LogInformation("New Order Service {id}", id);
    }

    private const decimal TAX = 0.06M;
    public async Task<PostOrderResponse> PlaceOrderAsync(PostOrderRequest req, CancellationToken ct = default)
    {
        var custId = req.CustomerId;

        //make sure the cust exists / is valid
        var customer = await customerRepository.GetByIdAsync(req.CustomerId);
        if (customer is null) throw new InvalidOperationException("Customer does not exist");


        var productIds = req.OrderItems.Select(oi => oi.ProductId);

        var products = await productRepository.GetByIdsAsync(productIds);

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

        total = total * (1.00M + TAX);

        var newOrder = new Order
        {
            CustomerId = req.CustomerId,          
            OrderDate = req.OrderDate,
            OrderItems = req.OrderItems.Select(x => new OrderItem { ProductId = x.ProductId, Quantity = x.Quantity }).ToList(),
        };
        //add the order to the order repo

        await orderRepository.AddAsync(newOrder);


        await invoiceRepository.AddAsync(new Invoice
        {
            OrderId = newOrder.Id,
            CustomerId = customer.Id,
            Status = InvoiceStatuses.Unpaid.Status,
            Amount = total,
            InvoiceDt = DateTime.UtcNow

        });


        await shipmentRepository.AddAsync(new Shipment
        {
            OrderId = newOrder.Id,
            AddressId = customer.AddressId,
            Status = ShipmentStatuses.PendingPayment.Status
        });


        await orderRepository.SaveChangesAsync();

        return new PostOrderResponse(newOrder.Id);
    }
}
