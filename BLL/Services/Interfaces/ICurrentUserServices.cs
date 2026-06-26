using E_commerce.DAL.Entities.Users;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface ICurrentUserService
    {
        public string? UserId { get; }

        public string? Role { get; }

        public string? Email { get; }
        public Task<ApplicationUser?> GetCurrentUser();

        bool IsInRole(string role);


    }
}
