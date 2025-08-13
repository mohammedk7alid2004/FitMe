using FitMe.Contracts.Payment;
using Stripe;
using static System.Net.WebRequestMethods;
using Stripe.V2;
using System.Numerics;
using System.Net.Http.Json;
using FitMe.Contracts.Order;
namespace FitMe.Services
{
    public class PaymentServices : IPaymentServices
    {
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public PaymentServices(IConfiguration config, HttpClient http)
        {
            _config = config;
            _http = http;
            StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];
        }

        public PaymentIntent CreatePayment(PaymentRequest paymentRequest, IConfiguration config)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = paymentRequest.Amount,
                Currency = paymentRequest.Currency,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                },
            };
            var service = new PaymentIntentService();
            return service.Create(options);
        }

        public async  Task<string> CreatePaymentToken (PaymentRequest2 request)
        {
            var settings = _config.GetSection("PaymobSettings");

            var authResponse = await _http.PostAsJsonAsync(
                $"{settings["BaseUrl"]}/auth/tokens",
                new { api_key = settings["ApiKey"] }
            );
            var authData = await authResponse.Content.ReadFromJsonAsync<AuthResponse>();
            var authToken = authData.Token;

            var orderResponse = await _http.PostAsJsonAsync(
                $"{settings["BaseUrl"]}/ecommerce/orders",
                new
                {
                    auth_token = authToken,
                    delivery_needed = "false",
                    amount_cents = (int)(request.Amount * 100),
                    currency = "EGP",
                    items = new object[] { }
                }
            );
            var orderData = await orderResponse.Content.ReadFromJsonAsync<OrderResponse>();
            var orderId = orderData.id;

            var paymentKeyResponse = await _http.PostAsJsonAsync(
                $"{settings["BaseUrl"]}/acceptance/payment_keys",
            new
            {
                    auth_token = authToken,
                    amount_cents = (int)(request.Amount * 100),
                    expiration = 3600,
                    order_id = orderId,
                    billing_data = new
                    {
                        first_name = request.FirstName,
                        last_name = request.LastName,
                        email = request.Email,
                    },
                    currency = "EGP",
                    integration_id = settings["IntegrationId"]
                }
            );
            var paymentKeyData = await paymentKeyResponse.Content.ReadFromJsonAsync<PaymentKeyResponse>();
            return paymentKeyData.Token;
        }
    }
    }

