namespace FitMe.Errors;

public class CategoryError
{
    public static readonly Error DuplicatedCategoryName = new Error(
        "Category.DuplicatedCategoryName",
        "A Category with this name already exists.",
        StatusCodes.Status409Conflict
    );
    public static readonly Error CategoryNotFound = new Error(
       "Category.CategoryNotFound",
       "A Category is not found",
       StatusCodes.Status404NotFound
   );
}
