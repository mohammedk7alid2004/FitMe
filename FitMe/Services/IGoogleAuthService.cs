namespace FitMe.Services;

public interface IGoogleAuthService
{
    Task<Result<AuthResponse>> AuthenticateAsync(string idToken);

}
