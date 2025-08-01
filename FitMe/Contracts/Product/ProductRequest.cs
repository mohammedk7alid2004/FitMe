namespace FitMe.Contracts.Product;

public record ProductRequest
(
    string Name,
    string Description,
    IFormFile ImageUrl,
    decimal Price,
    string Size,
    decimal ? Rating,
    int Stock,
    int CategoryId,
    int BrandId
);

