namespace FitMe.Errors;

public static class ProductError
{
    public static readonly Error ProductNotFound = new Error(
        "ProductNotFound",
        "The requested product was not found.",
        StatusCodes.Status404NotFound
    );

    public static readonly Error ProductAlreadyExists = new Error(
        "ProductAlreadyExists",
        "A product with the same details already exists.",
        StatusCodes.Status409Conflict
    );
    public static readonly Error ProductPhotoNotUploaded = new Error(
     "ProductPhotoNotUploaded",
     "Failed to upload the product image.",
     StatusCodes.Status500InternalServerError
 );

}
