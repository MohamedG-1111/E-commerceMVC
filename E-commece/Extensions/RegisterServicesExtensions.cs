using BLL.Services.Implementation;
using BLL.Services.Interfaces;
using DataAccessLayer.Repositories.Implementation;
using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Implementation;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.DAL.Repositories.Implementation;
using E_commerce.DAL.Repositories.Interfaces;

namespace E_commece.Extensions
{
    public static class RegisterServicesExtensions
    {
        public static IServiceCollection AddDependenics(this IServiceCollection services)
        {
            services.AddControllersWithViews();


            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAttachmentService, AttachmentService>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IRedisService, RedisService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPricingService, PricingService>();

            return services;
        }
    }
}
