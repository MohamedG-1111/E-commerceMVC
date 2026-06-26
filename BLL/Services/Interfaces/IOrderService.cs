using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<Result<CheckoutViewModel>> GetCheckoutData();

        public Task<Result> PlaceOrderAsync();

        public Task<Result<IEnumerable<OrderVM>>> GetMyOrdersAsync();
        public Task<Result<OrderDetailsVM>> GetOrderDetails(int OrderId);
    }
}
