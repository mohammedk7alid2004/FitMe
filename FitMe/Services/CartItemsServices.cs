using FitMe.Contracts.Cart;
using FitMe.Contracts.Payment;
using FitMe.Models;
using FitMe.Persistence;
using System.Collections.Generic;

namespace FitMe.Services;

public class CartItemsServices(ApplicationDbContext context ,IPaymentServices paymentServices) : ICartItemsServices
{
    private readonly ApplicationDbContext _context = context;
    private readonly IPaymentServices _paymentService = paymentServices;

    public async Task<Result> AddAsync(CartItemsRequest request, string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(userId))
            return Result.Failure(UserErrors.UserIdNotnull);

        if (await GetOrCreateUserCartAsync(userId, cancellationToken) is not { } cart)
            return Result.Failure(CartError.FailedCart);

        if (await _context.Products.AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            is not { } product)
            return Result.Failure(ProductError.ProductNotFound);

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.CartId == cart.CartId && c.ProductId == request.ProductId, cancellationToken);

        if (cartItem is not null)
        {
            cartItem.Quantity += request.Quantity;

        }

        else
        {
            await _context.CartItems.AddAsync(new CartItems
            {
                CartId = cart.CartId,
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Price = product.Price,
            }, cancellationToken);
        }

        cart.UpdatedOn = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
    public async Task<Result<IEnumerable<CartItemsResponse>>> GetAllAsync(string userId, CancellationToken cancellationToken = default)
    {
        var items = await _context.Cart
         .Include(c => c.Items)
        .ThenInclude(i => i.Product)
        .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (items == null)
            return Result.Failure<IEnumerable<CartItemsResponse>> (CartError.CartNotFound);

        var response = items.Items.Select(i => new CartItemsResponse(
        i.Product!.Id,
        i.Product.Name,            
        i.Product.ImageUrl,        
        i.Quantity,
        i.Price,
        i.TotalPrice
    ));
        return Result.Success(response);
    }

    public async Task<Result<decimal>> GetTotalPriceAsync(string userId, CancellationToken cancellationToken = default)
    {
        var item = _context.Cart
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (item == null)
            return Result.Failure<decimal>(CartError.CartNotFound);
        decimal TotalPrice = item.Result.Items.Sum(i => i.TotalPrice);
        return Result.Success<decimal>(TotalPrice);

    }


    public async Task<Result> ClearCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (await _context.Cart.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken)is not { } cart)
            return Result.Failure(CartError.CartNotFound);

        await _context.CartItems
            .Where(ci => ci.CartId == cart.CartId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success();
    }


    public async Task<Result<bool>> DeleteAsync(int cartId, int productId, string userId, CancellationToken cancellationToken = default)
    {
        var deletedCount = await _context.CartItems
            .Where(x => x.CartId == cartId
                     && x.ProductId == productId
                     && x.Cart!.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return deletedCount > 0
            ? Result.Success(true)
            : Result.Failure<bool>(CartError.ProductNotFoundInCart);
    }



    public async Task<Result<int>> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        var totalQuantity = await _context.CartItems
            .Where(ci => ci.Cart!.UserId == userId)
            .SumAsync(ci => (int?)ci.Quantity, cancellationToken) ?? 0;

        return totalQuantity > 0
            ? Result.Success(totalQuantity)
            : Result.Failure<int>(CartError.CartNotFound);
    }


    public async Task<Result<bool>> UpdateAsync(CartItemUpdate request, string userId, CancellationToken cancellationToken = default)
    {
        if (request.Quantity == 0)
        {
            await DeleteAsync(0, request.ProductId, userId, cancellationToken); 
            return Result.Success(true);
        }

        var updatedCount = await _context.CartItems
            .Where(x => x.ProductId == request.ProductId && x.Cart!.UserId == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.Quantity, request.Quantity), cancellationToken);

        return updatedCount > 0
            ? Result.Success(true)
            : Result.Failure<bool>(CartError.ProductNotFoundInCart);
    }

    public async Task<Result<string>> MakeOrderAsync(string userId, PaymentRequest2 request, CancellationToken cancellationToken = default)
    {
        if (await GetCartItemsForOrder(userId, cancellationToken) is not { } cartItems || !cartItems.Any())
            return Result.Failure<string>(CartError.CartNotFound);

        var order = new Order
        {
            UserId = userId,
            Status = "Pending",
            OrderDetails = cartItems.Select(i => new OrderDetail
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.Price,
                TotalPrice = i.TotalPrice
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        var amount = cartItems.Sum(i => i.TotalPrice);
        var paymentToken = await _paymentService.CreatePaymentToken(request with { Amount = amount });

        await _context.CartItems
            .Where(c => c.Cart!.UserId == userId)
            .ExecuteDeleteAsync(cancellationToken);

        return Result.Success(paymentToken);
    }




    private async Task<Cart?> GetOrCreateUserCartAsync(string userId, CancellationToken cancellationToken)
    {
        var cart = await _context.Cart
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart is null)
        {
            cart = new Cart { UserId = userId };
            await _context.Cart.AddAsync(cart, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return cart;
    }
    private async Task<List<CartItemsResponse>> GetCartItemsForOrder(string userId, CancellationToken cancellationToken)
    {
            return await _context.CartItems
                .Where(ci => ci.Cart.UserId == userId)
             .Select(ci => new CartItemsResponse(
                ci.ProductId,
                ci.Product.Name,
                ci.Product.ImageUrl, 
                ci.Quantity,
                ci.Product.Price,
                ci.TotalPrice
            )).ToListAsync(cancellationToken);
    }


}
