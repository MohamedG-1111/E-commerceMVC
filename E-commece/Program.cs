using BLL.Services.Implementation;
using BLL.Services.Interfaces;
using DataAccessLayer.Repositories.Implementation;
using DataAccessLayer.Repositories.Interfaces;
using DataAcessLayer.Data;
using E_commerce.BLL.Services.Implementation;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.DAL.Data;
using E_commerce.DAL.Entities.Users;
using E_commerce.DAL.Repositories.Implementation;
using E_commerce.DAL.Repositories.Interfaces;
using E_commerce.DAL.Seeding;
using Ecommerce.Utility.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_commece
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IAttachmentService, AttachmentService>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<ICompanyService, CompanyService>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            builder.Services.Configure<IdentityOptions>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            });
            builder.Services.Configure<EmailSettings>(
     builder.Configuration.GetSection("EmailSettings"));
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            #region SeedingRoles && Admin
            var scope = app.Services.CreateScope();
            var IdentityRoleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            await SeedingRoles.SeedRolesAsync(IdentityRoleManager);

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await AdminSeeder.SeedAdminAsync(userManager);
            #endregion
            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
