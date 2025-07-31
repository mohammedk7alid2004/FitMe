namespace FitMe.Errors;

public class BrandError
{
    public static readonly Error DuplicatedBrandName = new Error(
        "Brand.DuplicatedBrandName",
        "A brand with this name already exists.",
        StatusCodes.Status409Conflict
    );
    public static readonly Error BrandNotFound = new Error(
       "Brand.BrandNotFound",
       "A brand is not found",
       StatusCodes.Status409Conflict
   );
}
