namespace FitMe.Contracts.Authentication;

public record RefreshTokenRequest(
   string Token,
   string RefreshToken    
 );
