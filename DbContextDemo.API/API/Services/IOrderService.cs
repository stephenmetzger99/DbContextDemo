using DbContextDemo.API.API.Features.Orders.PostOrder;

namespace DbContextDemo.API.API.Services;

public interface IOrderService
{
    Task<Guid> PlaceOrderAsync(PostOrderRequest order, CancellationToken ct = default);
}
