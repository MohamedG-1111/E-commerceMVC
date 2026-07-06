using BLL.Services.Interfaces;
using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities;
using Ecommerce.Utility;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
using Microsoft.EntityFrameworkCore;
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

            var cart = await redisService.GetAsync<CustomerCart>(userId);

            if (cart is null)
            {
                cart = new CustomerCart
                {
                    UserId = userId,
                    Items = new List<CartItem>()
                };
            }

            if (cart.Items.Any(x => x.ProductId == model.ProductId))
                return Result<CustomerCart>.Failure(
                    "Product already exists in cart",
                    errorType: ErrorType.CONFLICT);

            cart.Items.Add(new CartItem
            {
                ProductId = product.Value.Id,
                Name = product.Value.Title,
                Image = product.Value.ImageUrl,
                Count = model.quantity,
                Price = await PriceForProductAsync(model.quantity, product.Value)
            });

            return await CreateOrUpdateCartAsync(cart);
        }

        // ================= CreateOrUpdateCartAsync  =================
        public async Task<Result<CustomerCart>> CreateOrUpdateCartAsync(
            CustomerCart cart,
            TimeSpan? ttl = null)
        {
            if (cart is null)
                return Result<CustomerCart>.Failure(
                    "Cart is null",
                    errorType: ErrorType.VALIDATION);

            if (string.IsNullOrEmpty(cart.UserId))
                return Result<CustomerCart>.Failure(
                    "Invalid cart key",
                    errorType: ErrorType.VALIDATION);

            cart.Items ??= new List<CartItem>();

            cart = await CalculateCartAsync(cart);

            var result = await redisService.SetAsync(
                cart.UserId,
                cart,
                ttl ?? DefaultTTL);

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


        public async Task<Result<CustomerCart>> ClearCartAsync(string? UserId = null)
        {
            var userId = UserId ?? currentUser.UserId;

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


        public async Task<Result<CustomerCart>> RefreshCartAsync()
        {
            var cartResult = await GetAsync();

            if (cartResult.IsFailure)
                return cartResult;

            var cart = cartResult.Value;

            if (cart.Items == null || !cart.Items.Any())
            {
                return Result<CustomerCart>.Failure(
                    "Cart is empty",
                    errorType: ErrorType.VALIDATION);
            }

            var productIds = cart.Items
                .Select(x => x.ProductId)
                .ToList();

            var products = await unitOfWork.Repository<Product>()
         .GetAsQuery()
         .Where(x => productIds.Contains(x.Id))
         .ToDictionaryAsync(x => x.Id);
            foreach (var item in cart.Items)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                {
                    return Result<CustomerCart>.Failure(
                        $"Product ({item.Name}) not found",
                        errorType: ErrorType.NOT_FOUND);
                }

                if (product.Stock < item.Count)
                {
                    return Result<CustomerCart>.Failure(
                        $"{product.Title} doesn't have enough stock. Available Book {product.Stock}",
                        errorType: ErrorType.CONFLICT);
                }

                item.Name = product.Title;
                item.Image = product.ImageUrl;
                item.Price = await PriceForProductAsync(item.Count, product);
            }

            return await CreateOrUpdateCartAsync(cart);
        }


        // ================= PRICE =================
        public async Task<int> PriceForProductAsync(int quantity, Product product)
        {
            return quantity switch
            {
                <= 50 => product.PriceFor1To50,
                <= 100 => product.PriceFor50Plus,
                _ => product.PriceFor100Plus
            };
        }

        public async Task<CustomerCart> CalculateCartAsync(CustomerCart cart)
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