namespace FitMe.Contracts.Cart;

public record CartItemsResponse
(
     int CartId,
    int ProductId,
    int Quantity,
    decimal Price,
    
);

