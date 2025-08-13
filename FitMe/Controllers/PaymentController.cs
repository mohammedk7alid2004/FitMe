using FitMe.Contracts.Payment;
using FitMe.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitMe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController(IPaymentServices paymentServices,IConfiguration configuration) : ControllerBase
    {
        private readonly IPaymentServices _paymentServices = paymentServices;
        private readonly IConfiguration _configuration = configuration;
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest2 request)
        {
            var token = await _paymentServices.CreatePaymentToken(request);

            return Ok(new { PaymentToken = token });
        }
        [HttpPost("create-payment-intent")]
        public IActionResult CreatePaymentIntent([FromBody] PaymentRequest request)
        {
            var paymentIntent = _paymentServices.CreatePayment(request, _configuration);

            return Ok(new
            {
                clientSecret = paymentIntent.ClientSecret
            });
        }
    }
}
