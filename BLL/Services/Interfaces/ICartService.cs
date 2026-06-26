using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface ICartService
    {
        Task<Result<CustomerCart>> AddProductToCart(AddToCartViewModel model);

        Task<Result<CustomerCart>> CreateCartItemAsync(
            string key,
            CartItem item,
            TimeSpan? TTL = null);

        Task<Result<CustomerCart>> DeleteProductFromCartAsync(int productId);

        Task<Result<CustomerCart>> GetAsync();

        Task<Result<CustomerCart>> UpdateQuantityAsync(int productId, int change);

        public Task<Result<CustomerCart>> ClearCartAsync();
    }
}