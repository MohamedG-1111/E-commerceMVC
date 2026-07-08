using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Pagination;
using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IOrderService
    {
        public Task<Result<CheckoutViewModel>> GetCheckoutData();

        public Task<Result> PlaceOrderAsync();

        public Task<Result<PaginatedResult<OrderVM>>> GetOrdersAsync(PaginationParameters Parameters, OrderFilter? filter = null);
        public Task<Result<OrderDetailsVM>> GetOrderDetails(int OrderId);
        public Task<Result<OrderDetailsForAdminVM>> GetOrderDetailsForAdmin(int OrderId);

        public Task<Result> UpdateOrderStatus(UpdateOrderStatus updateOrderStatus);
    }
}
