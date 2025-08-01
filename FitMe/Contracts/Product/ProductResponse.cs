namespace FitMe.Contracts.Product;

public record ProductResponse
(
    int  Id,
   string Name,
    string Description,
    string ImageUrl, 
    decimal Price,
    string Size,
    decimal Rating,
    int Stock,
    int CategoryId,
    int BrandId
);
