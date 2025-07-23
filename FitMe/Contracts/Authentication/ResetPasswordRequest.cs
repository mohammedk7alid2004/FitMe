namespace FitMe.Contracts.Authentication;

public record ResetPasswordRequest
(
    string Email,
    string Code,
    string newPassword
);
