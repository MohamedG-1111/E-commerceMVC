using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Result;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface IAccountService
    {
        public Task<Result> ConfirmEmailAsync(string UserId, string token);

        public Task<Result> SendEmailConfirmationAsync(string emailTo, string confirmationLink);
        public Task<Result> ReSendEmailConfirmationAsync(string emailTo);

        public Task<Result<string>> GetUserEmail(string UserId);

        public Task<Result> SendResetPasswordEmailAsync(string email);

        //Task<Result> ValidateResetPasswordTokenAsync(string email, string token);

        public Task<Result> ResetPasswoAsync(ResetPasswordViewModel model);
    }
}
