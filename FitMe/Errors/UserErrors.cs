
namespace FitMe.Errors;
using FitMe.Abstractions;
public static class UserErrors
{
    public static readonly Error InvalidCredentials = new("User.InvalidCredentials", "Invalid email or password", StatusCodes.Status401Unauthorized);
    public static readonly Error EmailIsRequired =
        new("User.EmailIsRequired", "Email is required", StatusCodes.Status400BadRequest);
    public static readonly Error EmailNotFound =
        new("User.EmailNotFound", "Email not found", StatusCodes.Status404NotFound);
    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);
    public static readonly Error InvalidGoogleToken =
      new ("User.InvalidGoogleToken", "InvalidGoogleToken", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);
    public static readonly Error DuplicatedEmail =
        new("User.DuplicatedEmail", "another user with this email ", StatusCodes.Status409Conflict);
  
    public static readonly Error EmailNotConfirmed =
        new("User.EmailNotConfirmed", "Email is not confirmed", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidCode =
        new("User.InvalidCode", "Invalid code", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicatedConfirmation =
        new("User.DuplicatedConfirmation", "Email already confirmed", StatusCodes.Status400BadRequest);

}

