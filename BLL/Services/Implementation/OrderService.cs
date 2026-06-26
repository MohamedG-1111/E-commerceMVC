using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities;
using E_commerce.DAL.Entities.enums;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
using Microsoft.EntityFrameworkCore;
using Order = E_commerce.DAL.Entities.Order;
namespace E_commerce.BLL.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly ICartService cartService;
        private readonly ICurrentUserService currentUserService;
        private readonly IUnitOfWork unitOfWork;

        public OrderService(ICartService cartService,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            this.cartService = cartService;
            this.currentUserService = currentUserService;
            this.unitOfWork = unitOfWork;
        }
        public async Task<Result<CheckoutViewModel>> GetCheckoutData()
        {
            var currentUser = await currentUserService.GetCurrentUser();

            if (currentUser == null)
                return Result<CheckoutViewModel>.Failure(
                    "Must Login First",
                    errorType: ErrorType.UNAUTHORIZED);

            var customerCart = await cartService.GetAsync();

            if (customerCart.IsFailure)
                return Result<CheckoutViewModel>.Failure(
                    customerCart.ErrorMessage!,
                    errorType: customerCart.ErrorType);

            if (!customerCart.Value.Items.Any())
                return Result<CheckoutViewModel>.Failure(
                    "Cart is empty",
                    errorType: ErrorType.VALIDATION);

            var UpdateCheckoutInfoVM = new UpdateCheckoutInfoVM()
            {
                FirstName = currentUser.FirstName,
                LastName = currentUser.LastName,
                PhoneNumber = currentUser.PhoneNumber!,
                StreetAddress = currentUser.StreetAddress,
                City = currentUser.City,
                PostalCode = currentUser.PostalCode,
            };

            var checkoutVm = new CheckoutViewModel
            {
                UpdateCheckoutInfo = UpdateCheckoutInfoVM,
                Items = customerCart.Value.Items,
                SubTotal = customerCart.Value.SubTotal,
                Discount = customerCart.Value.Discount,
                Total = customerCart.Value.Total
            };

            return Result<CheckoutViewModel>.Success(checkoutVm);
        }

        public async Task<Result<IEnumerable<OrderVM>>> GetMyOrdersAsync()
        {
            var userId = currentUserService.UserId;
            if (userId == null)
                return Result<IEnumerable<OrderVM>?>.Failure(
                    "Must be logged in", errorType: ErrorType.UNAUTHORIZED);

            var orders = await unitOfWork.Repository<Order>()
                .GetAsQuery()
                .Where(x => x.ApplicationUserId == userId).
                OrderByDescending(x => x.OrderDate)
                .Select(x => new OrderVM
                {
                    Id = x.Id,
                    OrderTotal = x.OrderTotal,
                    OrderDate = x.OrderDate,
                    OrderStatus = x.OrderStatus.ToString(),
                    PaymentStatus = x.PaymentStatus.ToString(),
                    ItemsCount = x.OrderDetails.Sum(d => d.Count)
                })
                .ToListAsync();

            if (!orders.Any())
                return Result<IEnumerable<OrderVM>>.Failure(
                    "There are no orders yet",
                    errorType: ErrorType.NOT_FOUND);

            return Result<IEnumerable<OrderVM>?>.Success(orders);
        }

        public async Task<Result<OrderDetailsVM>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0)
                return Result<OrderDetailsVM>.Failure(
                    "Order Not Found",
                    errorType: ErrorType.NOT_FOUND);

            var userId = currentUserService.UserId;

            if (userId == null)
                return Result<OrderDetailsVM>.Failure(
                    "Must be Login",
                    errorType: ErrorType.UNAUTHORIZED);

            var orderDetails = await unitOfWork.Repository<Order>()
                .GetAsQuery()
                .Where(x => x.Id == orderId && x.ApplicationUserId == userId)
                .Select(x => new OrderDetailsVM
                {
                    Id = x.Id,
                    OrderTotal = x.OrderTotal,
                    OrderDate = x.OrderDate,
                    OrderStatus = x.OrderStatus.ToString(),
                    PaymentStatus = x.PaymentStatus.ToString(),

                    Items = x.OrderDetails
                        .Select(d => new OrderDetailsItemVM
                        {
                            ProductId = d.ProductId,
                            ProductName = d.Product.Title,
                            Image = d.Product.ImageUrl,
                            Price = d.Price,
                            Count = d.Count
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (orderDetails == null)
                return Result<OrderDetailsVM>.Failure(
                    "Order Not Found",
                    errorType: ErrorType.NOT_FOUND);

            return Result<OrderDetailsVM>.Success(orderDetails);
        }
        public async Task<Result> PlaceOrderAsync()
        {
            var currentUser = await currentUserService.GetCurrentUser();

            if (currentUser == null)
                return Result.Failure("Must Login First",
                    errorType: ErrorType.UNAUTHORIZED);

            var customerCart = await cartService.GetAsync();

            if (customerCart.IsFailure)
                return Result.Failure(customerCart.ErrorMessage!,
                    errorType: customerCart.ErrorType);

            if (!customerCart.Value.Items.Any())
                return Result.Failure("Cart is empty",
                    errorType: ErrorType.VALIDATION);

            var order = new Order
            {
                ApplicationUserId = currentUser.Id,
                OrderTotal = customerCart.Value.Total,
                PaymentStatus = currentUser.CompanyId != null
                    ? PaymentStatus.ApprovedForDelayedPayment
                    : PaymentStatus.Pending,

                OrderStatus = currentUser.CompanyId != null
                    ? OrderStatus.Approved
                    : OrderStatus.Pending
            };

            await unitOfWork.Repository<Order>().AddAsync(order);

            foreach (var item in customerCart.Value.Items)
            {
                var product = await unitOfWork.Repository<Product>()
                                              .GetByIdAsync(item.ProductId);

                if (product == null)
                    return Result.Failure("Product not found",
                        errorType: ErrorType.NOT_FOUND);

                if (product.Stock < item.Count)
                    return Result.Failure($"{product.Title} doesn't have enough stock.",
                        errorType: ErrorType.VALIDATION);

                product.Stock -= item.Count;

                order.OrderDetails.Add(new OrderDetails
                {
                    ProductId = item.ProductId,
                    Count = item.Count,
                    Price = item.Price
                });
            }

            int result = await unitOfWork.SaveChangesAsync();

            if (result <= 0)
                return Result.Failure("Failed to place order",
                    errorType: ErrorType.INTERNAL_ERROR);

            await cartService.ClearCartAsync();

            return Result.Success();
        }
    }
}


