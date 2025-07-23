namespace FitMe.Contracts.Users
{
    public class UpdateProfileValidator:AbstractValidator<UpdateProfileRequest>
   {
        public UpdateProfileValidator()
        {
            RuleFor(u=>u.FullName)
                .NotEmpty()
                .Length(3,200);

        }
    }
}
