using E_commerce.DAL.Data;
using E_commerce.DAL.Entities.Users;
using E_commerce.DAL.Seeding;
using Microsoft.AspNetCore.Identity;

namespace E_commece.Extensions
{

    public static class SeedingExtensions
    {
        public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            await SeedingRoles.SeedRolesAsync(roleManager);

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            await AdminSeeder.SeedAdminAsync(userManager);

            return app;
        }
    }
}
