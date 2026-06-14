using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<Result> ConfirmEmailAsync(string UserId, string token);
    }
}
