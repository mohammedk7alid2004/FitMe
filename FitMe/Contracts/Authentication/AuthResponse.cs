namespace FitMe.Contracts.Authentication;

public record AuthResponse   
(
    string Id,
    string ? Email,
    string UserName,
    string Token,
    int ExpiresIn,
    string RefreshToken,
    DateTime RefreshTokenExpiration
);

