using FitMe.Contracts.Cart;
using FitMe.Contracts.Payment;
using FitMe.Models;
using System.Collections.Generic;

namespace FitMe.Services;

public class CartItemsServices(ApplicationDbContext context ,IPaymentServices paymentServices) : ICartItemsServices
{
    private readonly ApplicationDbContext _context = context;
    private readonly IPaymentServices _paymentService = paymentServices;

    public async Task<Result> AddAsync(CartItemsRequest cartItemsRequest, string userId, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Cart
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart == null)
        {
            await CreateCart(userId, cancellationToken);
            cart = await _context.Cart
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken); 
        }

        var existingItem = await _context.CartItems
            .FirstOrDefaultAsync(c => c.Cart!.UserId == userId && c.ProductId == cartItemsRequest.ProductId, cancellationToken);

        if (existingItem != null)
        {
            existingItem.Quantity += cartItemsRequest.Quantity;
        }
        else
        {
            var product = await _context.Products.FindAsync(new object[] { cartItemsRequest.ProductId }, cancellationToken);
            if (product == null)
                return Result.Failure(ProductError.ProductNotFound);

            var cartItem = cartItemsRequest.Adapt<CartItems>();
            cartItem.CartId = cart.CartId; 
            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }


    public async Task<Result> ClearCartAsync(string userId, CancellationToken cancellationToken = default)
    {
        var cart = await _context.Cart
            .Include(c => c.Items)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (cart == null)
            return Result.Failure(CartError.CartNotFound);

        _context.CartItems.RemoveRange(cart.Items);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }


    public async Task<Result> CreateCart(string userId, CancellationToken cancellationToken = default)
    {
        var cart = new Cart
        {
            UserId = userId,
            Items = new List<CartItems>()
        };
        _context.Cart.Add(cart);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();

    }

    public async Task<Result<bool>> DeleteAsync(int cartId, int productId, string userId, CancellationToken cancellationToken = default)
    {


        var cartItem = await _context.CartItems
            .Where(x => x.CartId == cartId && x.ProductId == productId && x.Cart.UserId == userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (cartItem == null)
            return Result.Failure<bool>(CartError.ProductNotFoundInCart);

        try
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success(true);
        }
        catch (DbUpdateException ex)
        {
            return Result.Failure<bool>(CartError.FailedToDeleteCartItems);
        }
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
        i.Price
    ));
        return Result.Success(response);
    }

    public async  Task<Result<int>> GetCartItemCountAsync(string userId, CancellationToken cancellationToken = default)
    {
        var item = _context.Cart
         .Include(c => c.Items)
         .ThenInclude(i => i.Product)
         
         .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken)
        ;
        if (item == null)
            return Result.Failure<int>(CartError.CartNotFound);
        int TotalPrice =  item.Result.Items.Select(x=>x.Quantity).Sum();
        return Result.Success<int>(TotalPrice);
    }

    public async Task<Result<decimal>> GetTotalPriceAsync(string userId, CancellationToken cancellationToken = default)
    {
        var item= _context.Cart
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
        if (item == null)
            return Result.Failure<decimal>(CartError.CartNotFound);
        decimal TotalPrice =item.Result.Items.Sum(i => i.TotalPrice);
        return Result.Success< decimal>(TotalPrice);

    }

   
    public async Task<Result<bool>> UpdateAsync(CartItemUpdate cartItemsRequest, string userId, CancellationToken cancellationToken = default)
    {
        var cartItem = await _context.CartItems.
             FirstOrDefaultAsync(x => x.ProductId == cartItemsRequest.ProductId && x.Cart!.UserId == userId, cancellationToken);
        if (cartItem == null)
            return Result.Failure<bool>(CartError.ProductNotFoundInCart);
        cartItem.Quantity = cartItemsRequest.Quantity;
        _context.CartItems.Update(cartItem);
        if(cartItem.Quantity == 0)
        {
             DeleteAsync(cartItem.CartId, cartItemsRequest.ProductId, userId, cancellationToken);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success<bool>(true);

    }
    public async Task<Result<string>> MakeOrderAsync(string userId, PaymentRequest2 request2, CancellationToken cancellationToken = default)
    {
        var cartResult = await GetAllAsync(userId, cancellationToken);
        if (!cartResult.IsSuccess)
            return Result.Failure<string>(cartResult.Error);

        var cartItems = cartResult.Value;
        if (!cartItems.Any())
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
                TotalPrice = i.Price * i.Quantity
            }).ToList()
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        request2.Amount = cartItems.Sum(i => i.Price * i.Quantity);

        var paymentToken = await _paymentService.CreatePaymentToken( request2);

        await ClearCartAsync(userId, cancellationToken);

        return Result.Success(paymentToken);
    }

}
