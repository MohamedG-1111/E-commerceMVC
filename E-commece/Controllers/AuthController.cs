using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class AuthController : AppController
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Register(RegisterationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await authService.RegisterAsync(model);
            if (result.IsFailure)
                return HandleResult(result, nameof(Register), model);
            else
                TempData["Success"] = "Registration successful !";
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await authService.LoginAsync(model);
            if (result.IsFailure)
                return HandleResult(result, nameof(Login), model);


            TempData["Success"] = "Login successful!";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            var result = await authService.LogOutAsync();
            if (result.IsFailure)
                HandleResult(result);
            TempData["Success"] = "Logout successful!";
            return RedirectToAction("Index", "Home");
        }
    }
}
