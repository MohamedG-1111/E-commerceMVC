using DataAcessLayer.Data;
using E_commerce.DAL.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace E_commece.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
            });
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Auth/Login";
            });
            return services;
        }
    }
}
