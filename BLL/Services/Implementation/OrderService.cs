using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities;
using E_commerce.DAL.Entities.enums;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;

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
                PaymentStatus = currentUser.CompanyId != 0
                    ? PaymentStatus.ApprovedForDelayedPayment
                    : PaymentStatus.Pending,

                OrderStatus = currentUser.CompanyId != 0
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


