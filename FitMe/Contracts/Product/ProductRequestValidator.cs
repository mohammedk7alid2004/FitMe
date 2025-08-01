namespace FitMe.Contracts.Product;

public class ProductRequestValidator:AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");
        RuleFor(x=>x.Price)
            .NotEmpty()
            .GreaterThan(0).WithMessage("Price must be greater than 0.");
        RuleFor(x => x.Size)
            .MaximumLength(10).WithMessage("Size must not exceed 10 characters.");

    }
}
