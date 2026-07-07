using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<Result<CustomerCart>> CreateOrUpdatePaymentIntentAsync();
        Task RefundPaymentAsync(string paymentIntentId);
        Task HandleWebhookAsync(string json, string stripeSignature);
        public Task<Result<RetryPaymentClientSecretVM>> CreateRetryPaymentAsync(int orderId);
        Task<Result<OrderPaymentVM>> RetryPaymentAsync(int orderId);
    }
}