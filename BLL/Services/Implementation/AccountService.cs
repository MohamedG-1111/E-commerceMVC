using E_commerce.BLL.Dto;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities.Users;
using Ecommerce.Utility;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace E_commerce.BLL.Services.Implementation
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(UserManager<ApplicationUser> _userManager, IHttpContextAccessor httpContextAccessor, IEmailService emailService)
        {
            this._userManager = _userManager;
            this.emailService = emailService;
            _httpContextAccessor = httpContextAccessor;

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

        public async Task<Result<string>> GetUserEmail(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Result<string>.Failure("UserId is required", errorType: ErrorType.NOT_FOUND);

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return Result<string>.Failure("User not found", errorType: ErrorType.NOT_FOUND);

            if (string.IsNullOrWhiteSpace(user.Email))
                return Result<string>.Failure("Email not found", errorType: ErrorType.NOT_FOUND);

            return Result<string>.Success(user.Email);
        }

        public async Task<Result> ReSendEmailConfirmationAsync(string emailTo)
        {
            if (string.IsNullOrWhiteSpace(emailTo))
                return Result.Failure("Email is not found", errorType: ErrorType.NOT_FOUND);

            var user = await _userManager.FindByEmailAsync(emailTo);

            if (user == null)
                return Result.Failure("User is not found", errorType: ErrorType.NOT_FOUND);

            if (user.EmailConfirmed)
                return Result.Failure("Email already confirmed", errorType: ErrorType.VALIDATION);


            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var request = _httpContextAccessor.HttpContext!.Request;

            var confirmationLink =
            $"{request.Scheme}://{request.Host}/Account/ConfirmEmail?userId={user.Id}&token={encodedToken}";

            // 👇 reuse نفس الميثود
            return await SendEmailConfirmationAsync(user.Email!, confirmationLink);
        }

        public async Task<Result> ResetPasswoAsync(ResetPasswordViewModel model)
        {
            if (model == null || model.Email == null || model.Password == null)
                return Result.Failure("InValid Data", errorType: ErrorType.VALIDATION);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Result.Failure("User not found", errorType: ErrorType.NOT_FOUND);

            var result = await _userManager.ResetPasswordAsync(
                user,
                model.Token,
                model.Password);
            await _userManager.UpdateSecurityStampAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result.Failure(errors, errorType: ErrorType.VALIDATION);
            }

            return Result.Success();


        }

        public async Task<Result> SendEmailConfirmationAsync(string emailTo, string confirmationLink)
        {
            if (string.IsNullOrWhiteSpace(emailTo))
                return Result.Failure(
                     "Email is not Found",
                    errorType: ErrorType.NOT_FOUND);

            if (string.IsNullOrWhiteSpace(confirmationLink))
                return Result.Failure(
                    "Confirmation link is not Found",
                    errorType: ErrorType.NOT_FOUND);

            var body = EmailTemplateBuilder.BuildActionEmail(
     "Welcome to E-Commerce",
     "Thank you for registering with us. Please confirm your email address to activate your account.",
     "Confirm Email",
     confirmationLink
 );

            var emailRequest = new EmailRequestDto(
                emailTo,
                "Verify your E-Commerce account email",
                body);


            var result = await emailService.SendEmailAsync(emailRequest);
            return result.IsSuccess
                 ? Result.Success()
                 : Result.Failure("Failed to send email", errorType: ErrorType.INTERNAL_ERROR);
        }

        public async Task<Result> SendResetPasswordEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return Result.Failure("Email Is Required", errorType: ErrorType.VALIDATION);

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Failure("Invaild Email Adddress", errorType: ErrorType.VALIDATION);


            if (!user.EmailConfirmed)
                return Result.Failure("Must Confirm Email First", errorType: ErrorType.VALIDATION);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);

            var request = _httpContextAccessor.HttpContext!.Request;

            var resetLink =
                $"{request.Scheme}://{request.Host}/Account/ResetPassword?email={user.Email}&token={encodedToken}";

            var body = EmailTemplateBuilder.BuildActionEmail(
            "Reset Password",
               "Click below to reset your password",
            "Reset Password",
           resetLink
);

            var emailRequest = new EmailRequestDto(
                user.Email!,
                "Reset Password",
                body);

            var result = await emailService.SendEmailAsync(emailRequest);
            return result.IsSuccess
                 ? Result.Success()
                 : Result.Failure("Failed to send email", errorType: ErrorType.INTERNAL_ERROR);
        }

        //public async Task<Result> ValidateResetPasswordTokenAsync(string email, string token)
        //{
        //    if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
        //        return Result.Failure("Invalid reset link", errorType: ErrorType.VALIDATION);

        //    var user = await _userManager.FindByEmailAsync(email);

        //    if (user == null)
        //        return Result.Failure("Invalid request");

        //    var decodedToken = Uri.UnescapeDataString(token);

        //    var isValid = await _userManager.VerifyUserTokenAsync(
        //        user,
        //        TokenOptions.DefaultProvider,
        //        "ResetPassword",
        //        decodedToken);

        //    return isValid
        //        ? Result.Success()
        //        : Result.Failure("Invalid or expired reset link", errorType: ErrorType.VALIDATION);
        //}
    }
}


