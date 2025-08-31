using DbContextDemo.API.API.Features.Orders.PostOrder;

namespace DbContextDemo.API.Application.Services.Interfaces;

public interface IOrderService
{
    Task<PostOrderResponse> PlaceOrderAsync(PostOrderRequest order, CancellationToken ct = default);
}
