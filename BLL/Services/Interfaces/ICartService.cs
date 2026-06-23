using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface ICartService
    {
        Task<Result<CustomerCart>> AddProductToCart(AddToCartViewModel model);

        Task<Result<CustomerCart>> CreateOrUpdateCartItemAsync(
            string key,
            CartItem item,
            TimeSpan? TTL = null);

        Task<Result> DeleteProductFromCartAsync(int productId);

        Task<Result<CustomerCart>> GetAsync();

        Task<Result<CustomerCart>> UpdateQuantityAsync(int productId, int quantity);
    }
}