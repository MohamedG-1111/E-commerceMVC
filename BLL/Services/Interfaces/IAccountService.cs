using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<Result<string>> ConfirmEmailAsync(string UserId, string token);

        public Task<Result> SendEmailConfirmationAsync(
        string emailTo,
        string confirmationLink);
    }
}
