//using DataAccessLayer.Repositories.Interfaces;
//using E_commerce.BLL.Services.Interfaces;
//using E_commerce.BLL.ViewModels;
//using Ecommerce.Utility.Result;
//using Ecommerce.Utility.ResultPattern;
//using Microsoft.Extensions.Configuration;
//using Stripe;
//using Product = E_commerce.DAL.Entities.Product;

//namespace E_commerce.BLL.Services.Implementation
//{
//    public class PaymentService : IPaymentService
//    {
//        private readonly ICartService _cartService;
//        private readonly IConfiguration config;
//        private readonly IUnitOfWork unitOfWork;

//        public PaymentService(ICartService cartService, IConfiguration config, IUnitOfWork unitOfWork)
//        {
//            _cartService = cartService;
//            this.config = config;
//            this.unitOfWork = unitOfWork;
//        }
//        public async Task<Result<CustomerCart>> CreateOrUpdatePaymentIntent()
//        {
//            // 0] Install stripe nuget package
//            // 1] Configure stripe settings in appsettings.json
//            StripeConfiguration.ApiKey = config.GetSection("StripeSettings:SecretKey").Value;
//            // 2] Get the customer cart from the database


//        }
//    }
//}
