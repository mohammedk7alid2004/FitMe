using FitMe.Abstractions.Const;

namespace FitMe.Contracts.Users
{
    public class ChangePasswordRequestValidator:AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.currentPassword)
                .NotEmpty();
            RuleFor(x=>x.newPassword)
                .NotEmpty()
                .NotEmpty().Matches(RegexPatterns.Password)
            .WithMessage("Password must be at least 8 characters long and include: 1 uppercase, 1 lowercase, 1 number, and 1 special character.")
            .NotEqual(x=>x.currentPassword)
            .WithMessage("new password cannot the same current password...");

        }
    }
}
