using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities.enums;
using Ecommerce.Utility.Result;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Stripe;
using OrderEntity = E_commerce.DAL.Entities.Order;
using ProductEntity = E_commerce.DAL.Entities.Product;

namespace E_commerce.BLL.Services.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly ICartService _cartService;
        private readonly IConfiguration _config;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            ICartService cartService,
            IConfiguration config,
            IUnitOfWork unitOfWork)
        {
            _cartService = cartService;
            _config = config;
            _unitOfWork = unitOfWork;
        }

        // =========================
        // CREATE / UPDATE PAYMENT INTENT
        // =========================
        public async Task<Result<CustomerCart>> CreateOrUpdatePaymentIntentAsync()
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var cartResult = await _cartService.RefreshCartAsync();

            if (!cartResult.IsSuccess)
            {
                return Result<CustomerCart>.Failure(
                    cartResult.ErrorMessage,
                    errorType: cartResult.ErrorType);
            }

            var cart = cartResult.Value;

            var totalAmount = (long)(cart.Total * 100);

            var paymentIntentService = new PaymentIntentService();

            if (string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions
                {
                    Amount = totalAmount,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                };

                var intent = await paymentIntentService.CreateAsync(options);

                cart.PaymentIntentId = intent.Id;
                cart.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = totalAmount
                };

                await paymentIntentService.UpdateAsync(cart.PaymentIntentId, options);
            }

            return await _cartService.CreateOrUpdateCartAsync(cart);
        }

        // =========================
        // WEBHOOK ENTRY POINT
        // =========================
        public async Task HandleWebhookAsync(string json, string stripeSignature)
        {
            var stripeEvent = EventUtility.ConstructEvent(
                json,
                stripeSignature,
                _config["StripeSettings:WebhookSecret"],
                throwOnApiVersionMismatch: false);

            switch (stripeEvent.Type)
            {
                case EventTypes.PaymentIntentSucceeded:
                    var successIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (successIntent != null)
                        await HandlePaymentSucceededAsync(successIntent.Id);
                    break;

                case EventTypes.PaymentIntentPaymentFailed:
                    var failedIntent = stripeEvent.Data.Object as PaymentIntent;
                    if (failedIntent != null)
                        await HandlePaymentFailedAsync(failedIntent.Id);
                    break;

                case EventTypes.ChargeRefunded:
                    var charge = stripeEvent.Data.Object as Charge;
                    if (charge?.PaymentIntentId != null)
                        await HandleChargeRefundedAsync(charge.PaymentIntentId);
                    break;
            }
        }

        // =========================
        // REFUND PAYMENT
        // =========================
        public async Task RefundPaymentAsync(string paymentIntentId)
        {
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var paymentIntentService = new PaymentIntentService();
            var intent = await paymentIntentService.GetAsync(paymentIntentId);

            if (intent == null)
                throw new InvalidOperationException("PaymentIntent not found.");

            if (intent.Status != "succeeded")
                throw new InvalidOperationException("Payment not completed.");

            if (intent.LatestCharge == null)
                throw new InvalidOperationException("Charge not found.");

            var refundService = new RefundService();

            var refund = await refundService.CreateAsync(new RefundCreateOptions
            {
                Charge = intent.LatestCharge.Id,
                Reason = RefundReasons.RequestedByCustomer
            });

            if (refund.Status != "succeeded")
                throw new InvalidOperationException("Refund failed.");
        }

        // =========================
        // PAYMENT SUCCESS
        // =========================
        private async Task HandlePaymentSucceededAsync(string paymentIntentId)
        {
            var order = await _unitOfWork.Repository<OrderEntity>()
                .GetAsQuery(false)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.PaymentIntentId == paymentIntentId);

            if (order == null || order.PaymentStatus == PaymentStatus.Paid)
                return;

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var item in order.OrderDetails)
                {
                    var updated = await _unitOfWork.Repository<ProductEntity>()
                        .GetAsQuery(false)
                        .Where(p => p.Id == item.ProductId && p.Stock >= item.Count)
                        .ExecuteUpdateAsync(p => p.SetProperty(
                            x => x.Stock,
                            x => x.Stock - item.Count));

                    if (updated == 0)
                    {
                        order.PaymentStatus = PaymentStatus.Rejected;
                        order.OrderStatus = OrderStatus.Cancelled;

                        _unitOfWork.Repository<OrderEntity>().Update(order);
                        await _unitOfWork.SaveChangesAsync();

                        await transaction.RollbackAsync();

                        if (!string.IsNullOrEmpty(order.PaymentIntentId))
                            await RefundPaymentAsync(order.PaymentIntentId);

                        return;
                    }
                }

                order.PaymentStatus = PaymentStatus.Paid;
                order.OrderStatus = OrderStatus.Approved;

                _unitOfWork.Repository<OrderEntity>().Update(order);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();

                order.PaymentStatus = PaymentStatus.Rejected;
                order.OrderStatus = OrderStatus.Cancelled;

                _unitOfWork.Repository<OrderEntity>().Update(order);
                await _unitOfWork.SaveChangesAsync();

                if (!string.IsNullOrEmpty(order.PaymentIntentId))
                    await RefundPaymentAsync(order.PaymentIntentId);

                return;
            }

            await _cartService.ClearCartAsync(order.ApplicationUserId);
        }
        // =========================
        // PAYMENT FAILED
        // =========================
        private async Task HandlePaymentFailedAsync(string paymentIntentId)
        {
            var order = await _unitOfWork.Repository<OrderEntity>()
                .GetAsQuery(false)
                .FirstOrDefaultAsync(x => x.PaymentIntentId == paymentIntentId);

            if (order == null)
                return;

            if (order.PaymentStatus == PaymentStatus.Paid)
                return;

            order.PaymentStatus = PaymentStatus.Rejected;
            order.OrderStatus = OrderStatus.Cancelled;

            await _unitOfWork.SaveChangesAsync();
        }

        // =========================
        // REFUND EVENT FROM STRIPE
        // =========================
        private async Task HandleChargeRefundedAsync(string paymentIntentId)
        {
            var order = await _unitOfWork.Repository<OrderEntity>()
                .GetAsQuery(false)
                .FirstOrDefaultAsync(x => x.PaymentIntentId == paymentIntentId);

            if (order == null)
                return;

            order.PaymentStatus = PaymentStatus.Refunded;
            order.OrderStatus = OrderStatus.Cancelled;

            await _unitOfWork.SaveChangesAsync();
        }

        public Task RetryPaymentAsync(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}