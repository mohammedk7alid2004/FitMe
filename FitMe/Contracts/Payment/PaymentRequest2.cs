namespace FitMe.Contracts.Payment;

public record PaymentRequest2
{
    public decimal Amount { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
}
