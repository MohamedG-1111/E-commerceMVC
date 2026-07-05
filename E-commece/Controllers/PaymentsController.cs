using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class PaymentsController : AppController
    {
        public readonly IPaymentService paymentService;
        private readonly IConfiguration _configuration;

        public PaymentsController(IPaymentService paymentService, IConfiguration configuration)
        {
            this.paymentService = paymentService;
            _configuration = configuration;
        }

        public IActionResult Success()
        {
            return View();
        }


        public async Task<IActionResult> CreateOrUpdatePaymentIntent()
        {
            var result = await paymentService.CreateOrUpdatePaymentIntentAsync();
            if (!result.IsSuccess)
            {
                return HandleResult(result);
            }
            return View("Payment", new PaymentViewModel()
            {
                PublishableKey = _configuration["StripeSettings:PublishableKey"],
                ClientSecret = result.Value.ClientSecret
            });
        }
    }
}
