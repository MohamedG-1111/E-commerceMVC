using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class AccountController : AppController
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var resultEmail = await accountService.GetUserEmail(userId);

            if (resultEmail.IsSuccess)
                ViewBag.Email = resultEmail.Value;

            var result = await accountService.ConfirmEmailAsync(userId, token);

            if (result.IsFailure)
                return HandleResult(result, nameof(ConfirmEmail));

            TempData["success"] = "Email confirmed successfully!";
            return View();
        }
        public async Task<IActionResult> ResendEmailConfirmation(string Email)
        {
            var result = await accountService.ReSendEmailConfirmationAsync(Email);

            if (!result.IsSuccess)
                return HandleResult(result, nameof(ConfirmEmail));
            ViewBag.Email = Email;
            TempData["success"] = "Resend Email Confirmation successfully!";
            return View();

        }

        [HttpGet]
        public async Task<IActionResult> ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await accountService.SendResetPasswordEmailAsync(model.Email);
            if (!result.IsSuccess)
                return HandleResult(result, nameof(ForgotPassword), model);

            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
                return View("InvalidResetLink");

            return View(new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await accountService.ResetPasswoAsync(model);
            if (!result.IsSuccess)
                return HandleResult(result, nameof(ResetPassword), model);
            TempData["Success"] = "Reset Password Successfuly";
            return RedirectToAction("Login", "Auth");
        }
    }
}
