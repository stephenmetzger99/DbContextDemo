namespace DbContextDemo.API.API.Features.Orders.PostOrder;

public sealed record OrderItemRequest(Guid ProductId, int Quantity);
public sealed record PostOrderRequest(Guid CustomerId,
    IEnumerable<OrderItemRequest> OrderItems,
    DateTime OrderDate);

