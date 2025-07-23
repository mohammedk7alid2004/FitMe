using FitMe.Extensions;
using FitMe.Migrations;
using Microsoft.EntityFrameworkCore;

namespace FitMe.Services;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IJwtProvider jwtProvider,
    ILogger<AuthService> logger,
    IEmailSender emailSender,
    IHttpContextAccessor httpContextAccessor,
    IWebHostEnvironment env,
    ApplicationDbContext context) : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly ILogger<AuthService> _logger = logger;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IWebHostEnvironment _env = env;
    private readonly ApplicationDbContext _context = context;
    private readonly int _otpExpiryMinutes = 5;
    private readonly int _refreshTokenExpiryDays = 14;

    public async Task<Result<AuthResponse>> GetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidCredentials);
        if (!user.EmailConfirmed)
        {
            return Result.Failure<AuthResponse>(UserErrors.EmailNotConfirmed);
        }
        var result = await _signInManager.PasswordSignInAsync(user, password, false, false);

        if (result.Succeeded)
        {
            var (token, expiresIn) = _jwtProvider.GenerateToken(user);
            var refreshToken = GenerateRefreshToken();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            user.RefreshTokens.Add(new RefreshToken
            {
                Token = refreshToken,
                ExpiresOn = refreshTokenExpiration
            });

            await _userManager.UpdateAsync(user);

            var response = new AuthResponse(user.Id, user.Email, user.FullName, token, expiresIn, refreshToken, refreshTokenExpiration);

            return Result.Success(response);
        }

        return Result.Failure<AuthResponse>(result.IsNotAllowed ? UserErrors.EmailNotConfirmed : UserErrors.InvalidCredentials);
    }

    public async Task<Result<AuthResponse>> GetRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure<AuthResponse>(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user);
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        user.RefreshTokens.Add(new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        });

        await _userManager.UpdateAsync(user);

        var response = new AuthResponse(user.Id, user.Email, user.FullName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }

    public async Task<Result> RevokeRefreshTokenAsync(string token, string refreshToken, CancellationToken cancellationToken = default)
    {
        var userId = _jwtProvider.ValidateToken(token);

        if (userId is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var userRefreshToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken && x.IsActive);

        if (userRefreshToken is null)
            return Result.Failure(UserErrors.InvalidRefreshToken);

        userRefreshToken.RevokedOn = DateTime.UtcNow;

        await _userManager.UpdateAsync(user);

        return Result.Success();
    }

    public async Task<Result> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var emailIsExists = await _userManager.Users.AnyAsync(x => x.Email == request.Email, cancellationToken);

        if (emailIsExists)
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = request.Adapt<ApplicationUser>();
        user.UserName = request.Email;
        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await user.UploadPhotoAsync(request.Photo, _env, _userManager);

            await SendOtpAsync(user);
            
            return Result.Success(user.Id);
        }

        var error = result.Errors.First();
        
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> ConfirmEmailAsync(ConfirmEmailRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);
        var user1=await  _userManager.FindByEmailAsync(request.Email);
        var otpRecord = await _context.OTP
            .Where(x => x.UserId == user1!.Id  && x.Code == request.Code)
            .FirstOrDefaultAsync();

        if (otpRecord == null)
            return Result.Failure(UserErrors.InvalidCode);

        if (otpRecord.ExpiryTime < DateTime.UtcNow)
        {
            _context.OTP.Remove(otpRecord);
            await _context.SaveChangesAsync();
            return Result.Failure(UserErrors.InvalidCode);
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var result = await _userManager.ConfirmEmailAsync(user, token);

        if (result.Succeeded)
        {
            _context.OTP.Remove(otpRecord);
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
    }

    public async Task<Result> SendResetPasswordAsync(string email)
    {
        if (await _userManager.FindByEmailAsync(email) is not { } user)
            return Result.Success();

        await SendPasswordResetOtpAsync(user);

        return Result.Success();
    }
    public async Task<Result> SendResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user is null || !user.EmailConfirmed)
            return Result.Failure(UserErrors.InvalidCode);

        var otpRecord = await _context.OTP
            .Where(x => x.UserId == user.Id && x.Code == request.Code)
            .FirstOrDefaultAsync();

        if (otpRecord == null || otpRecord.ExpiryTime < DateTime.UtcNow)
        {
            if (otpRecord != null)
            {
                _context.OTP.Remove(otpRecord);
                await _context.SaveChangesAsync();
            }

            return Result.Failure(UserErrors.InvalidCode);
        }

        _context.OTP.Remove(otpRecord);
        await _context.SaveChangesAsync();

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, resetToken, request.newPassword);

        if (result.Succeeded)
        {
            return Result.Success();
        }

        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));

    }
    public async Task<Result> ResendConfirmationEmailAsync(ResendConfirmationEmailRequest request)
    {
        if (await _userManager.FindByEmailAsync(request.Email) is not { } user)
            return Result.Success();

        if (user.EmailConfirmed)
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        await SendOtpAsync(user);

        return Result.Success();
    }

    private static string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    private async Task<Result> SendOtpAsync(ApplicationUser user)
    {
        var otpCode = new Random().Next(100000, 999999).ToString();

        var existingOtps = await _context.OTP
            .Where(x => x.UserId == user.Id)
            .ToListAsync();

        if (existingOtps.Any())
        {
            _context.OTP.RemoveRange(existingOtps);
        }

        var otp = new OTP
        {
            Code = otpCode,
            ExpiryTime = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes),
            UserId = user.Id,
            User = user
        };

        _context.OTP.Add(otp);
        await _context.SaveChangesAsync();

        var emailBody = EmailBodyBuilder.GenerateEmailBody("EmailConfirmation",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FullName },
                { "{{otp_code}}", otpCode },
                { "{{expiry_minutes}}", _otpExpiryMinutes.ToString() }
            }
        );

        await _emailSender.SendEmailAsync(user.Email!, "✅ FitMe: Email Verification OTP", emailBody);

        _logger.LogInformation("OTP sent to user {UserId}: {OtpCode}", user.Id, otpCode);

        return Result.Success();
    }

    private async Task<Result> SendPasswordResetOtpAsync(ApplicationUser user)
    {
        var otpCode = new Random().Next(100000, 999999).ToString();

        var existingOtps = await _context.OTP
            .Where(x => x.UserId == user.Id)
            .ToListAsync();

        if (existingOtps.Any())
        {
            _context.OTP.RemoveRange(existingOtps);
        }

        var otp = new OTP
        {
            Code = otpCode,
            ExpiryTime = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes),
            UserId = user.Id,
            User = user
        };

        _context.OTP.Add(otp);
        await _context.SaveChangesAsync();

        var emailBody = EmailBodyBuilder.GenerateEmailBody("ForgetPassword",
            templateModel: new Dictionary<string, string>
            {
                { "{{name}}", user.FullName },
                { "{{otp_code}}", otpCode },
                { "{{expiry_minutes}}", _otpExpiryMinutes.ToString() }
            }
        );

        await _emailSender.SendEmailAsync(user.Email!, "🔐 FitMe: Password Reset OTP", emailBody);

        _logger.LogInformation("Password reset OTP sent to user {UserId}: {OtpCode}", user.Id, otpCode);

        return Result.Success();
    }

   
}