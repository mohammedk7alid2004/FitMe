using FitMe.Contracts.Cart;
using FitMe.Contracts.Payment;
using FitMe.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitMe.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class CartController(ICartItemsServices cartItems) : ControllerBase
{
    private readonly ICartItemsServices _cartItems = cartItems;
    [HttpGet("")]
    public async Task<IActionResult> GetAll()
    {
        var userId = User.GetUserId();
        var result = await _cartItems.GetAllAsync(userId);

       return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPost("")]
    public async Task<IActionResult> AddToCart([FromBody] CartItemsRequest cartItemsRequest)
    {
        var userId = User.GetUserId();
        var result = await _cartItems.AddAsync(cartItemsRequest, userId);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpGet("total-price")]
    public async Task<IActionResult> GetTotalPrice()
    {
        var userId = User.GetUserId();
        var result = await _cartItems.GetTotalPriceAsync(userId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("count")]
    public async Task<IActionResult> GetCartItemCount()
    {
        var userId = User.GetUserId();
        var result = await _cartItems.GetCartItemCountAsync(userId);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpDelete]
    public async Task<IActionResult> DeleteFromCart([FromQuery] int cartId, [FromQuery] int productId)
    {
        var userId = User.GetUserId();
        var result = await _cartItems.DeleteAsync(cartId, productId, userId);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPut]
    public async Task<IActionResult> UpdateCartItem([FromBody] CartItemUpdate cartItemsRequest)
    {
        var userId = User.GetUserId();
        var result = await _cartItems.UpdateAsync(cartItemsRequest, userId!);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart()
    {
        var userId = User.GetUserId();
        var result = await _cartItems.ClearCartAsync(userId);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPost("make-order")]
    public async Task<IActionResult> MakeOrder([FromBody] PaymentRequest2 request2)
    {
        var userId = User.GetUserId();
        var result = await _cartItems.MakeOrderAsync(userId!, request2);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
}   }    
