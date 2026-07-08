using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities.enums;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
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
        private readonly ICurrentUserService currentUserService;
        private readonly IPricingService pricingService;

        public PaymentService(
            ICartService cartService,
            IConfiguration config,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService, IPricingService pricingService)
        {
            _cartService = cartService;
            _config = config;
            _unitOfWork = unitOfWork;
            this.currentUserService = currentUserService;
            this.pricingService = pricingService;
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
                Charge = intent.LatestChargeId,
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

            if (order == null)
                return;

            if (order.PaymentStatus == PaymentStatus.Paid ||
                order.PaymentStatus == PaymentStatus.Refunded)
            {
                return;
            }
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
                        await transaction.RollbackAsync();
                        order.OrderStatus = OrderStatus.Cancelled;
                        _unitOfWork.Repository<OrderEntity>().Update(order);
                        await _unitOfWork.SaveChangesAsync();
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

                order.OrderStatus = OrderStatus.Cancelled;

                _unitOfWork.Repository<OrderEntity>().Update(order);
                await _unitOfWork.SaveChangesAsync();

                if (!string.IsNullOrEmpty(order.PaymentIntentId))
                    await RefundPaymentAsync(order.PaymentIntentId);

                return;
            }

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
            order.PaymentDate = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
        }

        // =========================
        // REFUND EVENT FROM STRIPE
        // =========================
        private async Task HandleChargeRefundedAsync(string paymentIntentId)
        {
            var order = await _unitOfWork.Repository<OrderEntity>()
                .GetAsQuery()
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(x => x.PaymentIntentId == paymentIntentId);

            if (order == null)
                return;

            if (order.PaymentStatus == PaymentStatus.Refunded)
                return;

            await using var transaction = await _unitOfWork.BeginTransactionAsync();

            try
            {
                foreach (var item in order.OrderDetails)
                {
                    await _unitOfWork.Repository<ProductEntity>()
                        .GetAsQuery(false)
                        .Where(p => p.Id == item.ProductId)
                        .ExecuteUpdateAsync(x => x.SetProperty(
                            p => p.Stock,
                            p => p.Stock + item.Count));
                }

                order.PaymentStatus = PaymentStatus.Refunded;
                order.OrderStatus = OrderStatus.Cancelled;

                _unitOfWork.Repository<OrderEntity>().Update(order);

                await _unitOfWork.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<Result<OrderPaymentVM>> RetryPaymentAsync(int orderId)
        {
            var userId = currentUserService.UserId;

            if (string.IsNullOrEmpty(userId))
            {
                return Result<OrderPaymentVM>.Failure(
                    "Must login first",
                    errorType: ErrorType.UNAUTHORIZED);
            }

            var order = await _unitOfWork.Repository<OrderEntity>()
                .GetAsQuery(false)
                .Include(x => x.OrderDetails)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x =>
                    x.Id == orderId &&
                    x.ApplicationUserId == userId);

            if (order == null)
                return Result<OrderPaymentVM>.Failure("Order not found", errorType: ErrorType.NOT_FOUND);

            if (order.PaymentStatus == PaymentStatus.Paid)
                return Result<OrderPaymentVM>.Failure("Order already paid", errorType: ErrorType.CONFLICT);

            if (order.OrderStatus != OrderStatus.Pending)
                return Result<OrderPaymentVM>.Failure("Order cannot be paid", errorType: ErrorType.CONFLICT);

            foreach (var item in order.OrderDetails)
            {
                if (item.Product.Stock < item.Count)
                {
                    return Result<OrderPaymentVM>.Failure(
                        $"{item.Product.Title} doesn't have enough stock.",
                        errorType: ErrorType.CONFLICT);
                }

                item.Price = await pricingService.PriceForProductAsync(item.Count, item.Product);
            }

            var subTotal = order.OrderDetails.Sum(x => x.Price * x.Count);

            var (discountAmount, total) =
                await pricingService.CalculatePricingAsync(subTotal);

            order.OrderTotal = total;

            await _unitOfWork.SaveChangesAsync();

            return Result<OrderPaymentVM>.Success(new OrderPaymentVM
            {
                OrderId = order.Id,
                OrderTotal = total,
                Discount = discountAmount,
                Items = order.OrderDetails.Select(x => new OrderPaymentItemVM
                {
                    ProductId = x.ProductId,
                    ProductName = x.Product.Title,
                    Image = x.Product.ImageUrl,
                    Price = x.Price,
                    Count = x.Count
                }).ToList()
            });
        }


        public async Task<Result<RetryPaymentClientSecretVM>> CreateRetryPaymentAsync(int orderId)
        {
            var userId = currentUserService.UserId;
            if (userId == null)
                return Result<RetryPaymentClientSecretVM>.Failure(
                    "Must login first",
                    errorType: ErrorType.UNAUTHORIZED);
            StripeConfiguration.ApiKey = _config["StripeSettings:SecretKey"];

            var result = await RetryPaymentAsync(orderId);

            if (!result.IsSuccess)
            {
                return Result<RetryPaymentClientSecretVM>.Failure(
                    result.ErrorMessage,
                    errorType: result.ErrorType);
            }

            var order = await _unitOfWork.Repository<OrderEntity>()
                .GetAsQuery(false)
                .FirstAsync(x => x.Id == orderId && x.ApplicationUserId == userId);

            var paymentIntentService = new PaymentIntentService();

            var paymentIntent = await paymentIntentService.CreateAsync(
                new PaymentIntentCreateOptions
                {
                    Amount = (long)(order.OrderTotal * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }
                });

            order.PaymentIntentId = paymentIntent.Id;

            await _unitOfWork.SaveChangesAsync();

            return Result<RetryPaymentClientSecretVM>.Success(new RetryPaymentClientSecretVM
            {
                ClientSecret = paymentIntent.ClientSecret
            });
        }
    }
}