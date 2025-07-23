namespace FitMe.Contracts.Users;

public record ChangePasswordRequest
(
  string currentPassword,
  string newPassword
);
