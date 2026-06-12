using Ecommerce.Utility;
using Microsoft.AspNetCore.Identity;

namespace E_commerce.DAL.Data
{
    public static class SeedingRoles
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(Roles.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin));
            }
            if (!await roleManager.RoleExistsAsync(Roles.Customer))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Customer));
            }
            if (!await roleManager.RoleExistsAsync(Roles.Employee))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Employee));
            }
            if (!await roleManager.RoleExistsAsync(Roles.Company))
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.Company));
            }
        }
    }
}
