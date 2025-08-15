namespace FitMe.Contracts.Cart;

public record CartItemsResponse
(
    int ProductId,
    string ProductName,
    string? ProductImageUrl,
    int Quantity,
    decimal Price  ,
    decimal TotalPrice  
);

