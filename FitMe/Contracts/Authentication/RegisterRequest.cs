namespace FitMe.Contracts.Authentication;

public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public IFormFile? Photo { get; set; }
}