using E_commerce.BLL.Services.Interfaces;
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

        public async Task<IActionResult> ConfirmEmail(string userId, string Token)
        {
            var result = await accountService.ConfirmEmailAsync(userId, Token);
            if (result.IsFailure)
                return HandleResult(result, nameof(ConfirmEmail));
            TempData["success"] = "Email confirmed successfully!";
            return View();

        }
    }
}
