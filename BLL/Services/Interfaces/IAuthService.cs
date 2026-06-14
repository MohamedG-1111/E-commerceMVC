using E_commerce.BLL.Dto;
using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        public Task<Result<RegisterResultDto>> RegisterAsync(RegisterationViewModel model);
        public Task<Result> LoginAsync(LoginViewModel model);
        public Task<Result> LogOutAsync();
    }
}
