
using FitMe.Contracts.Email;

namespace FitMe.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(
    IAuthService authService,
    ILogger<AuthController> logger,
    IEmailSender emailSender,
    IGoogleAuthService authService1) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IGoogleAuthService _authService1 = authService1;

    [HttpPost("")]
    public async Task<IActionResult> Login([FromForm] LoginRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logging with email: {email} and password: {password}", request.Email, request.Password);

        var authResult = await _authService.GetTokenAsync(request.Email, request.Password, cancellationToken);

        return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();

       
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromForm] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var authResult = await _authService.GetRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return authResult.IsSuccess ? Ok(authResult.Value) : authResult.ToProblem();
    }

    [HttpPost("revoke-refresh-token")]
    public async Task<IActionResult> RevokeRefreshToken([FromForm] RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RevokeRefreshTokenAsync(request.Token, request.RefreshToken, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromForm] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.RegisterAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromForm] ConfirmEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ConfirmEmailAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("resend-confirmation-email")]
    public async Task<IActionResult> ResendConfirmationEmail([FromForm] ResendConfirmationEmailRequest request, CancellationToken cancellationToken)
    {
        var result = await _authService.ResendConfirmationEmailAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromForm] ForgetPasswordRequest request)
    {
        var result = await _authService.SendResetOtpAsync(request.Email);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPost("verify-reset-otp")]
    public async Task<IActionResult> VerifyResetOtp([FromForm] VerifyOtpRequest request)
    {
        var result = await _authService.VerifyResetOtpAsync(request.Email, request.Code);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok("OTP is valid.");
    }


    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromForm] GoogleAuthRequest request)
    {
        _logger.LogInformation("Google login attempt with token: {Token}", request.IdToken);

        var result = await _authService1.AuthenticateAsync(request.IdToken);

        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }



}