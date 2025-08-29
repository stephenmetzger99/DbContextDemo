using DbContextDemo.API.API.Features.Orders.PostOrder;

namespace DbContextDemo.API.API.Services;

public interface IOrderService
{
    Task<PostOrderResponse> PlaceOrderAsync(PostOrderRequest order, CancellationToken ct = default);
}
