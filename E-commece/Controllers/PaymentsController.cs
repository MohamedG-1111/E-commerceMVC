using E_commerce.BLL.Services.Interfaces;
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


        public async Task<IActionResult> Index()
        {
            var result = await paymentService.CreateOrUpdatePaymentIntentAsync();
            if (!result.IsSuccess)
            {
                return HandleResult(result);
            }
            return View(result.Value);
        }

        [HttpPost]
        public async Task<IActionResult> webhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];

            await paymentService.HandleWebhookAsync(json, stripeSignature);

            return Ok();
        }
    }
}
