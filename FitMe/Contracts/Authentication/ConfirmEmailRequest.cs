namespace FitMe.Contracts.Authentication;

public record ConfirmEmailRequest(
    string Email,
    string Code
);