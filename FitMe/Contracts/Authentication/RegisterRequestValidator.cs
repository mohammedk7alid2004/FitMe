using FitMe.Abstractions.Const;

namespace FitMe.Contracts.Authentication;

public class RegisterRequestValidator:AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty()
       .EmailAddress()
           ;
        RuleFor(x => x.Password)
            .NotEmpty().Matches(RegexPatterns.Password)
            .WithMessage("Password must be at least 8 characters long and include: 1 uppercase, 1 lowercase, 1 number, and 1 special character.");
        RuleFor(x => x.FullName)
            .Length(3,200).NotEmpty();
         
    }
}
