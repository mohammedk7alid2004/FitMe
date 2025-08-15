using FitMe.Abstractions;
using FitMe.Contracts.Cart;
using FitMe.Contracts.Payment;

namespace FitMe.Services;

public interface ICartItemsServices
{
    Task<Result<IEnumerable<CartItemsResponse>>> GetAllAsync(string userId,CancellationToken cancellationToken=default);
    Task<Result>AddAsync(CartItemsRequest cartItemsRequest, string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(CartItemUpdate cartItemsRequest, string userId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(int cartId, int productId, string userId, CancellationToken cancellationToken = default);
    Task<Result> ClearCartAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<decimal>> GetTotalPriceAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<int>> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default);
    Task<Result<string>> MakeOrderAsync(string userId,PaymentRequest2 request2, CancellationToken cancellationToken = default);

}
