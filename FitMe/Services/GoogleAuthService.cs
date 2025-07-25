using Google.Apis.Auth;

public class GoogleAuthService : IGoogleAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtProvider _jwtProvider;
    private readonly ILogger<GoogleAuthService> _logger;

    public GoogleAuthService(
        UserManager<ApplicationUser> userManager,
        IJwtProvider jwtProvider,
        ILogger<GoogleAuthService> logger)
    {
        _userManager = userManager;
        _jwtProvider = jwtProvider;
        _logger = logger;
    }

    public async Task<Result<AuthResponse>> AuthenticateAsync(string idToken)
    {
        GoogleJsonWebSignature.Payload payload;

        try
        {
            payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
        }
        catch (InvalidJwtException ex)
        {
            _logger.LogError(ex, "Invalid Google token.");
            return Result.Failure<AuthResponse>(UserErrors.InvalidGoogleToken);
        }

        var user = await _userManager.FindByEmailAsync(payload.Email);
        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName = payload.Email,
                Email = payload.Email,
                EmailConfirmed = true,
            };

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
            }
        }

        var (token, expiresIn) = _jwtProvider.GenerateToken(user);
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(7); // أو حسب عدد الأيام اللي انت محددها

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(
            user.Id,
            user.Email,
            user.FullName ?? user.UserName!,
            token,
            expiresIn,
            refreshToken,
            refreshTokenExpiration
        );

        return Result.Success(response);
    }
    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }


}
