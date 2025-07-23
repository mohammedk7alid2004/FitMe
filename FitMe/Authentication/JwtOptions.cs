using System.ComponentModel.DataAnnotations;

namespace FitMe.Authentication;

public class JwtOptions
{
    public static string SectionName = "Jwt";
    [Required]
    public string Key { get; init; }
    [Required]

    public string Issuer { get; init; }
    [Required]

    public string Audience { get; init; }
    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; init; }

}