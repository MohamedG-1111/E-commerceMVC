using E_commerce.BLL.Services.Interfaces;
using E_commerce.DAL.Entities.Users;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
using Microsoft.AspNetCore.Identity;

namespace E_commerce.BLL.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(UserManager<ApplicationUser> _userManager)
        {
            this._userManager = _userManager;
        }
        public async Task<Result> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return Result.Failure("Invalid confirmation link", errorType: ErrorType.VALIDATION);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure("User not found", errorType: ErrorType.NOT_FOUND);

            if (user.EmailConfirmed)
                return Result.Failure("Email already confirmed", errorType: ErrorType.VALIDATION);
            var decodedToken = Uri.UnescapeDataString(token);


            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
                return Result.Failure("Invalid or expired confirmation link", errorType: ErrorType.VALIDATION);

            }

            return Result.Success();
        }
    }
}
