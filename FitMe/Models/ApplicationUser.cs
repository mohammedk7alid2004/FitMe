
namespace FitMe.Models;

public class ApplicationUser: IdentityUser
{
    public string Photo { get; set; } = string.Empty;
    public string FullName { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<OTP>? Otps { get; set; }

}
