using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Result;
using Microsoft.Extensions.Configuration;
using Stripe;

namespace E_commerce.BLL.Services.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly ICartService _cartService;
        private readonly IConfiguration config;

        public PaymentService(ICartService cartService, IConfiguration config)
        {
            _cartService = cartService;
            this.config = config;
        }
        public async Task<Result<CustomerCart>> CreateOrUpdatePaymentIntent()
        {
            // 0] Install stripe nuget package
            // 1] Configure stripe settings in appsettings.json
            StripeConfiguration.ApiKey = config.GetSection("StripeSettings:SecretKey").Value;

            // 2] Refresh the cart from the database

            var cartResult = await _cartService.RefreshCartAsync();

            if (!cartResult.IsSuccess)
            {
                return Result<CustomerCart>.Failure(cartResult.ErrorMessage, errorType: cartResult.ErrorType);
            }
            // 3] Total amount to be charged in cents

            var totalAmount = (long)(cartResult.Value.Total * 100);

            // 4] Create or update the payment intent
            var paymentIntentService = new PaymentIntentService();
            if (string.IsNullOrEmpty(cartResult.Value.PaymentIntentId))
            {
                // Create a new payment intent
                var options = new PaymentIntentCreateOptions
                {
                    Amount = totalAmount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                };
                var paymentIntent = await paymentIntentService.CreateAsync(options);
                cartResult.Value.PaymentIntentId = paymentIntent.Id;
                cartResult.Value.ClientSecret = paymentIntent.ClientSecret;
            }
            else
            {
                // Update the existing payment intent
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = totalAmount,
                };
                await paymentIntentService.UpdateAsync(cartResult.Value.PaymentIntentId, options);
            }
            return await _cartService.CreateOrUpdateCartAsync(cartResult.Value);

        }
    }
}
