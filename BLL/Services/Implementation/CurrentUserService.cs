using System.Security.Claims;
using DataAcessLayer.Data;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.DAL.Entities.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.BLL.Services.Implementation
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly AppDbContext context;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor, AppDbContext context)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.context = context;
        }
        private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

        public string? UserId => User?.FindFirst(ClaimTypes.NameIdentifier)
            ?.Value ?? null;

        public string? Role => User?.FindFirst(ClaimTypes.Role)
            ?.Value ?? null;

        public string? Email => User?.FindFirst(ClaimTypes.Email)
            ?.Value ?? null;

        public async Task<ApplicationUser?> GetCurrentUser()
        {
            if (string.IsNullOrEmpty(UserId))
                return null;

            return await context.Users
                .FirstOrDefaultAsync(u => u.Id == UserId);
        }

        public bool IsInRole(string role)
        {
            return User?.IsInRole(role) ?? false;
        }
    }
}
