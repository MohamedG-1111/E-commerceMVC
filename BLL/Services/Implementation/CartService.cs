using BLL.Services.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;

namespace E_commerce.BLL.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IRedisService redisService;
        private readonly IProductService productService;
        private readonly ICurrentUserService currentUser;

        private static readonly TimeSpan DefaultTTL = TimeSpan.FromDays(7);

        public CartService(
            IRedisService redisService,
            IProductService productService,
            ICurrentUserService currentUser)
        {
            this.redisService = redisService;
            this.productService = productService;
            this.currentUser = currentUser;
        }

        // ================= ADD PRODUCT =================
        public async Task<Result<CustomerCart>> AddProductToCart(AddToCartViewModel model)
        {
            var userId = currentUser.UserId;

            if (string.IsNullOrEmpty(userId))
                return Result<CustomerCart>.Failure(
                    "Must Be Login",
                    errorType: ErrorType.UNAUTHORIZED);

            if (model is null || model.quantity <= 0)
                return Result<CustomerCart>.Failure(
                    "Invalid Data",
                    errorType: ErrorType.VALIDATION);

            var product = await productService.ProductDetailsAsync(model.ProductId);

            if (product?.Value is null)
                return Result<CustomerCart>.Failure(
                    "Product Not Found",
                    errorType: ErrorType.NOT_FOUND);

            var cartItem = new CartItem
            {
                ProductId = product.Value.Id,
                Name = product.Value.Title,
                Image = product.Value.ImageUrl,
                Count = model.quantity,
                Price = GetPrice(model.quantity, product.Value)
            };

            return await CreateOrUpdateCartItemAsync(userId, cartItem);
        }

        // ================= CREATE / UPDATE =================
        public async Task<Result<CustomerCart>> CreateOrUpdateCartItemAsync(
            string key,
            CartItem item,
            TimeSpan? TTL = null)
        {
            if (string.IsNullOrEmpty(key))
                return Result<CustomerCart>.Failure(
                    "Invalid cart key",
                    errorType: ErrorType.VALIDATION);

            if (item.Count <= 0)
                return Result<CustomerCart>.Failure(
                    "Invalid quantity",
                    errorType: ErrorType.VALIDATION);

            var cart = await redisService.GetAsync<CustomerCart>(key);

            if (cart is null)
            {
                cart = new CustomerCart
                {
                    UserId = key,
                    Items = new List<CartItem>()
                };
            }


            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == item.ProductId);

            if (existingItem is not null)
                existingItem.Count += item.Count;
            else
                cart.Items.Add(item);

            await redisService.SetAsync(key, cart, TTL ?? DefaultTTL);

            return Result<CustomerCart>.Success(cart);
        }

        // ================= DELETE PRODUCT =================
        public async Task<Result> DeleteProductFromCartAsync(int productId)
        {
            var key = currentUser.UserId;

            if (string.IsNullOrEmpty(key))
                return Result.Failure(
                    "Must Be Login",
                    errorType: ErrorType.UNAUTHORIZED);

            var cart = await redisService.GetAsync<CustomerCart>(key);

            if (cart is null)
                return Result.Failure(
                    "Cart not found",
                    errorType: ErrorType.NOT_FOUND);

            cart.Items ??= new List<CartItem>();

            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item is null)
                return Result.Failure(
                    "Product not found in cart",
                    errorType: ErrorType.NOT_FOUND);

            cart.Items.Remove(item);

            if (!cart.Items.Any())
                await redisService.DeleteAsync(key);
            else
                await redisService.SetAsync(key, cart, DefaultTTL);

            return Result.Success();
        }

        // ================= GET CART =================
        public async Task<Result<CustomerCart>> GetAsync()
        {
            var key = currentUser.UserId;

            if (string.IsNullOrEmpty(key))
                return Result<CustomerCart>.Failure(
                    "Must Be Login",
                    errorType: ErrorType.UNAUTHORIZED);

            var result = await redisService.GetAsync<CustomerCart>(key);

            if (result is null)
                return Result<CustomerCart>.Failure(
                    "Cart not found",
                    errorType: ErrorType.NOT_FOUND);

            return Result<CustomerCart>.Success(result);
        }

        // ================= UPDATE QUANTITY =================
        public async Task<Result<CustomerCart>> UpdateQuantityAsync(int productId, int quantity)
        {
            var key = currentUser.UserId;

            if (string.IsNullOrEmpty(key))
                return Result<CustomerCart>.Failure(
                    "Must Be Login",
                    errorType: ErrorType.UNAUTHORIZED);

            if (quantity < 0)
                return Result<CustomerCart>.Failure(
                    "Invalid quantity",
                    errorType: ErrorType.VALIDATION);

            var cart = await redisService.GetAsync<CustomerCart>(key);

            if (cart is null)
                return Result<CustomerCart>.Failure(
                    "Cart not found",
                    errorType: ErrorType.NOT_FOUND);

            cart.Items ??= new List<CartItem>();

            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);

            if (item is null)
                return Result<CustomerCart>.Failure(
                    "Product not found in cart",
                    errorType: ErrorType.NOT_FOUND);

            if (quantity == 0)
                cart.Items.Remove(item);
            else
                item.Count = quantity;

            if (!cart.Items.Any())
                await redisService.DeleteAsync(key);
            else
                await redisService.SetAsync(key, cart, DefaultTTL);

            return Result<CustomerCart>.Success(cart);
        }

        // ================= PRICE =================
        private int GetPrice(int quantity, Product product)
        {
            return quantity switch
            {
                <= 50 => product.PriceFor1To50,
                <= 100 => product.PriceFor50Plus,
                _ => product.PriceFor100Plus
            };
        }
    }
}