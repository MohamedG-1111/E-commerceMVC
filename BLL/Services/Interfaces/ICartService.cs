using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities;
using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface ICartService
    {
        Task<Result<CustomerCart>> AddProductToCart(AddToCartViewModel model);

        Task<Result<CustomerCart>> CreateOrUpdateCartAsync(
            CustomerCart cart,
            TimeSpan? ttl = null);

        Task<Result<CustomerCart>> DeleteProductFromCartAsync(int productId);

        Task<Result<CustomerCart>> GetAsync();

        Task<Result<CustomerCart>> UpdateQuantityAsync(int productId, int change);

        public Task<Result<CustomerCart>> ClearCartAsync();


        public Task<Result<CustomerCart>> RefreshCartAsync();
        public Task<int> PriceForProductAsync(int quantity, Product product);
        public Task<CustomerCart> CalculateCartAsync(CustomerCart cart);

    }
}