using E_commerce.BLL.Dto;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class AuthController : AppController
    {
        private readonly IAuthService authService;
        private readonly IAccountService accountService;

        public AuthController(
            IAuthService authService,
            IAccountService accountService)
        {
            this.authService = authService;
            this.accountService = accountService;
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Register

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterationViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await authService.RegisterAsync(model);

            if (result.IsFailure)
                return HandleResult(result, nameof(Register), model);

            var emailResult = await EmailConfirmation(result);

            if (emailResult.IsFailure)
            {
                TempData["Warning"] =
                    "Account created successfully, but the confirmation email could not be sent.";

                ViewBag.Email = model.Email;

                return View("EmailConfirmationRequired");
            }

            TempData["Success"] =
                "Registration successful. Please check your email to confirm your account.";

            return RedirectToAction(nameof(Login));
        }

        #endregion

        #region Login

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await authService.LoginAsync(model);

            if (result.IsFailure)
                return HandleResult(result, nameof(Login), model);

            TempData["Success"] = "Login successful!";

            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Logout

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var result = await authService.LogOutAsync();

            if (result.IsFailure)
                return HandleResult(result);

            TempData["Success"] = "Logout successful!";

            return RedirectToAction("Index", "Home");
        }

        #endregion

        private async Task<Result> EmailConfirmation(Result<RegisterResultDto> result)
        {
            var confirmationLink = Url.Action(
                "ConfirmEmail",
                "Account",
                new
                {
                    userId = result.Value.UserId,
                    token = result.Value.Token
                },
                Request.Scheme);

            return await accountService.SendEmailConfirmationAsync(
                result.Value.Email,
                confirmationLink!);
        }
    }
}