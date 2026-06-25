using BLL.Services.Interfaces;
using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities;
using Ecommerce.Utility;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;

namespace E_commerce.BLL.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IRedisService redisService;
        private readonly IProductService productService;
        private readonly ICurrentUserService currentUser;
        private readonly IUnitOfWork unitOfWork;
        private static readonly TimeSpan DefaultTTL = TimeSpan.FromDays(7);

        public CartService(
            IRedisService redisService,
            IProductService productService,
            ICurrentUserService currentUser, IUnitOfWork unitOfWork)
        {
            this.redisService = redisService;
            this.productService = productService;
            this.currentUser = currentUser;
            this.unitOfWork = unitOfWork;
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

            return await CreateCartItemAsync(userId, cartItem);
        }

        // ================= CREATE  =================
        public async Task<Result<CustomerCart>> CreateCartItemAsync(string key, CartItem item, TimeSpan? TTL = null)
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
                return Result<CustomerCart>.Failure(
                    "Product already exists in cart",
                    errorType: ErrorType.CONFLICT);

            cart.Items.Add(item);

            cart = await CalculateCartAsync(cart);

            var result = await redisService.SetAsync(
                key,
                cart,
                TTL ?? DefaultTTL);

            return Result<CustomerCart>.Success(result);
        }

        // ================= DELETE PRODUCT =================
        public async Task<Result<CustomerCart>> DeleteProductFromCartAsync(int productId)
        {
            if (productId <= 0)
                return Result<CustomerCart>.Failure("Product Not Found", errorType: ErrorType.NOT_FOUND);

            var key = currentUser.UserId;

            if (string.IsNullOrEmpty(key))
                return Result<CustomerCart>.Failure(
                    "Must Be Login",
                    errorType: ErrorType.UNAUTHORIZED);

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

            cart.Items.Remove(item);

            if (!cart.Items.Any())
            {
                await redisService.DeleteAsync(key);
            }
            else
            {
                cart = await CalculateCartAsync(cart);
                await redisService.SetAsync(key, cart, DefaultTTL);
            }
            return Result<CustomerCart>.Success(cart);
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
        public async Task<Result<CustomerCart>> UpdateQuantityAsync(int productId, int change)
        {
            var key = currentUser.UserId;

            if (string.IsNullOrEmpty(key))
                return Result<CustomerCart>.Failure(
                    "Must Be Login",
                    errorType: ErrorType.UNAUTHORIZED);



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

            item.Count += change;

            if (item.Count <= 0)
            {
                cart.Items.Remove(item);
            }



            if (!cart.Items.Any())
            {
                await redisService.DeleteAsync(key);
            }
            else
            {

                cart = await CalculateCartAsync(cart);
                await redisService.SetAsync(key, cart, DefaultTTL);
            }


            return Result<CustomerCart>.Success(cart);
        }


        public async Task<Result<CustomerCart>> ClearCartAsync()
        {
            var userId = currentUser.UserId;

            if (string.IsNullOrEmpty(userId))
                return Result<CustomerCart>.Failure(
                    "Must Login First",
                    errorType: ErrorType.UNAUTHORIZED);

            var cart = await redisService.GetAsync<CustomerCart>(userId);

            if (cart is null)
                return Result<CustomerCart>.Failure(
                    "Cart Not Found",
                    errorType: ErrorType.NOT_FOUND);

            var isDeleted = await redisService.DeleteAsync(userId);

            if (!isDeleted)
                return Result<CustomerCart>.Failure(
                    "Unable To Delete Cart",
                    errorType: ErrorType.INTERNAL_ERROR);

            return Result<CustomerCart>.Success(new CustomerCart
            {
                UserId = userId,
                Items = new List<CartItem>()
            });
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

        private async Task<CustomerCart> CalculateCartAsync(CustomerCart cart)
        {
            if (cart?.Items == null || !cart.Items.Any())
                return cart;

            var subTotal = cart.Items.Sum(x => x.Price * x.Count);
            var result = await CalculatePricingAsync(subTotal);

            cart.SubTotal = subTotal;
            cart.Discount = result.discountAmount;
            cart.Total = result.total;

            return cart;
        }
        private async Task<(decimal discountAmount, decimal total)> CalculatePricingAsync(decimal subtotal)
        {
            var user = await currentUser.GetCurrentUser();

            if (user == null || user.Role != Roles.Company)
                return (0, subtotal);

            var company = unitOfWork.Repository<Company>()
                .GetAsQuery()
                .FirstOrDefault(x => x.Id == user.CompanyId);

            if (company == null)
                return (0, subtotal);

            var discountPercent = company?.DiscountPercentage ?? 0;

            if (discountPercent <= 0)
                return (0, subtotal);

            var discountAmount = Math.Min(subtotal, subtotal * (discountPercent / 100m));
            var total = subtotal - discountAmount;

            return (discountAmount, total);
        }
    }
}