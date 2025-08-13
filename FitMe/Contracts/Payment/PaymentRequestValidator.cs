namespace FitMe.Contracts.Payment
{
    public class PaymentRequestValidator:AbstractValidator<PaymentRequest2>
    {
        public PaymentRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .Length(3, 100);
            RuleFor(x => x.LastName)
                .Length(3, 100);
            RuleFor(x => x.Email)
                .EmailAddress()
                .WithMessage("Invalid email format");
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Amount must be greater than zero");
        }
    }
}
