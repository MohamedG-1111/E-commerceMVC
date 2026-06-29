using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Pagination;
using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<Result<CheckoutViewModel>> GetCheckoutData();

        public Task<Result> PlaceOrderAsync();

        public Task<Result<PaginatedResult<OrderVM>>> GetMyOrdersAsync(PaginationParameters Parameters, OrderFilter? filter = null);
        public Task<Result<OrderDetailsVM>> GetOrderDetails(int OrderId);
    }
}
