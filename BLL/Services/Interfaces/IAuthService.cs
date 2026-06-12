using E_commerce.BLL.ViewModels;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<bool> RegisterAsync(RegisterationViewModel model);
        public Task<bool> LoginAsync(LoginViewModel model);
        public Task<bool> LogOutAsync();
    }
}
