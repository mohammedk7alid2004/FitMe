namespace FitMe.Errors;

public static class CartError
{
    public static readonly Error CartNotFound = new Error(
       "CartNotFound",
       "The requested Cart was not found.",
       StatusCodes.Status404NotFound
   );
    public static readonly Error ProductNotFoundInCart = new Error(
      "ProductNotFoundInCart",
      "The requested Product was not found.",
      StatusCodes.Status404NotFound
  );
    public static readonly Error FailedToDeleteCartItems = new Error(
     "FailedToDeleteCartItem",
     "FailedToDeleteCartItem.",
     StatusCodes.Status400BadRequest

 );


    public static readonly Error FailedCart =
        new("User.Failed Cart", "Failed to get or create cart", StatusCodes.Status400BadRequest);

}
