using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Pagination;
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

        public Task<Result> CreateAccountAsync(AccountVM model);


        public Task<Result<PaginatedResult<AllAccountsViewModel>>> GetAccountsAsync(
               PaginationParameters parameter,
               string? search = null);

        public Task<Result<AccountVM>?> GetAccountByUserId(string UserId);



        public Task<Result> LockAccountAsync(string UserId);
        public Task<Result> UnLockAccountAsync(string userId);

        public Task<Result> DeleteAccountAsync(string userId);

        public Task<Result> UpdateAccountAsync(string userId, EditAccountVM model);
        public Task<Result<EditAccountVM>> GetAccountToEditAsync(string userId);

        public Task<Result> UpdateCheckoutInfo(UpdateCheckoutInfoVM model);
    }
}
