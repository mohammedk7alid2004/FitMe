using FitMe.Contracts.Payment;
using Stripe;

namespace FitMe.Services
{
    public interface IPaymentServices
    {
        PaymentIntent CreatePayment(PaymentRequest paymentRequest, IConfiguration config);
        Task<string> CreatePaymentToken(PaymentRequest2 request);
    }
}
