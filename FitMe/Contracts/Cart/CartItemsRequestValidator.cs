namespace FitMe.Contracts.Cart;

public class CartItemsRequestValidator:AbstractValidator<CartItemsRequest>
{
    public CartItemsRequestValidator()
    {
            RuleFor(x=>x.Quantity).NotEmpty();
    }
}
