namespace FitMe.Contracts.Users
{
    public record UpdateProfileRequest
    (
        string FullName,
          IFormFile ? Photo
    );
}
