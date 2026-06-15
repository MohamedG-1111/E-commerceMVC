using E_commerce.BLL.Dto;
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
        private readonly IEmailService emailService;

        public AccountService(UserManager<ApplicationUser> _userManager, IEmailService emailService)
        {
            this._userManager = _userManager;
            this.emailService = emailService;
        }
        public async Task<Result<string>> ConfirmEmailAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return Result<string>.Failure("Invalid confirmation link", errorType: ErrorType.VALIDATION);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<string>.Failure("User not found", errorType: ErrorType.NOT_FOUND);

            if (user.EmailConfirmed)
                return Result<string>.Failure("Email already confirmed", errorType: ErrorType.VALIDATION);
            var decodedToken = Uri.UnescapeDataString(token);


            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine(error.Description);
                }
                return Result<string>.Failure("Invalid or expired confirmation link", errorType: ErrorType.VALIDATION);

            }

            return Result<string>.Success(user.Email!);
        }

        public async Task<Result> SendEmailConfirmationAsync(
        string emailTo,
        string confirmationLink)
        {
            if (string.IsNullOrWhiteSpace(emailTo))
                return Result.Failure(
                    "Email is required",
                    errorType: ErrorType.VALIDATION);

            if (string.IsNullOrWhiteSpace(confirmationLink))
                return Result.Failure(
                    "Confirmation link is required",
                    errorType: ErrorType.VALIDATION);

            var emailRequest = new EmailRequestDto
            (
                emailTo,
                "Verify your E-Commerce account email",
                $@"
<table width='100%' cellpadding='0' cellspacing='0' border='0' style='background-color:#f4f4f4;padding:40px 0;'>
    <tr>
        <td align='center'>

            <table width='520' cellpadding='0' cellspacing='0' border='0'
                   style='background-color:#ffffff;border-radius:10px;padding:30px;'>

                <tr>
                    <td align='center'>

                        <h1 style='margin:0 0 20px 0;color:#222;font-family:Arial,sans-serif;'>
                            Welcome to E-Commerce
                        </h1>

                        <p style='margin:0 0 25px 0;color:#555;font-size:16px;
                                  line-height:24px;font-family:Arial,sans-serif;'>
                            Thank you for registering with us.<br />
                            Please confirm your email address to activate your account.
                        </p>

                        <a href='{confirmationLink}'
                           style='background-color:#198754;
                                  color:#ffffff;
                                  text-decoration:none;
                                  padding:12px 24px;
                                  border-radius:6px;
                                  display:inline-block;
                                  font-family:Arial,sans-serif;
                                  font-weight:bold;'>
                            Confirm Email
                        </a>

                        <p style='margin-top:30px;color:#888;font-size:12px;
                                  font-family:Arial,sans-serif;'>
                            If you did not create this account, you can safely ignore this email.
                        </p>

                    </td>
                </tr>

            </table>

        </td>
    </tr>
</table>"
            );

            var result = await emailService.SendEmailAsync(emailRequest);
            if (result.IsSuccess)
                return Result.Success();

            return Result.Failure(result.ErrorMessage!);
        }
    }
}
