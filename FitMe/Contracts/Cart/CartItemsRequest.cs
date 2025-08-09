namespace FitMe.Contracts.Cart;

public record CartItemsRequest
(
    int CartId ,
    int ProductId,
    int Quantity,
    decimal Price,
    decimal TotalPrice
);

