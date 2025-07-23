namespace FitMe.Contracts.Users;

public record UserProfileResponse
(
    string Email,
    string FullName,
    string Photo
);
