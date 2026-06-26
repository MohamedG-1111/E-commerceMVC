using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class AccountController : AppController
    {
        private readonly IAccountService accountService;
        private readonly ICompanyService companyService;
        private readonly IOrderService orderService;

        public AccountController(IAccountService accountService, ICompanyService companyService, IOrderService orderService)
        {
            this.accountService = accountService;
            this.companyService = companyService;
            this.orderService = orderService;
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


        [HttpGet]

        public async Task<IActionResult> CreateAccount()
        {
            AccountVM model = new AccountVM()
            {
                Companies = await companyService.GetAllCompaniesItems()

            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(AccountVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Companies = await companyService.GetAllCompaniesItems();
                return View(model);

            }
            var result = await accountService.CreateAccountAsync(model);
            if (!result.IsSuccess)
            {
                model.Companies = await companyService.GetAllCompaniesItems();
                return HandleResult(result, nameof(CreateAccount), model);
            }

            TempData["Success"] = "Account Created Successfully";

            return RedirectToAction(nameof(Index));
        }



        public async Task<IActionResult> Index()
        {
            var result = await accountService.GetAccountsAsync();
            return View(result.Value);
        }

        public async Task<IActionResult> Profile(string UserId)
        {
            var result = await accountService.GetAccountByUserId(UserId);
            if (result.IsFailure)
                HandleResult(result);
            ViewBag.userId = UserId;
            return View(result.Value);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LockAccount(string userId)
        {
            var result = await accountService.LockAccountAsync(userId);

            if (result.IsFailure)
                return HandleResult(result);

            TempData["Success"] = "Account locked successfully";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnlockAccount(string userId)
        {
            var result = await accountService.UnLockAccountAsync(userId);

            if (result.IsFailure)
                return HandleResult(result);

            TempData["Success"] = "Account unlocked successfully";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string search)
        {
            var result = await accountService.SearchAccountsAsync(search);

            if (!result.IsSuccess)
            {
                return PartialView("_AccountPartial",
                    Enumerable.Empty<AllAccountsViewModel>());
            }

            return PartialView("_AccountPartial", result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Delete(string UserId)
        {
            var result = await accountService.DeleteAccountAsync(UserId);
            if (!result.IsSuccess)
                return HandleResult(result);
            TempData["Success"] = "Account Deleted Successfully";

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            var result = await accountService.GetAccountToEditAsync(userId);

            if (!result.IsSuccess || result.Value is null)
                return HandleResult(result);

            var model = result.Value;

            model.Companies = await companyService.GetAllCompaniesItems();

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(EditAccountVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Companies = await companyService.GetAllCompaniesItems();
                return View(model);
            }
            var result = await accountService.UpdateAccountAsync(model.UserId, model);
            if (!result.IsSuccess)
            {
                model.Companies = await companyService.GetAllCompaniesItems();
                return HandleResult(result, nameof(Edit), model);
            }

            TempData["Success"] = "Update Successfully";
            if (User.IsInRole(Roles.Admin))
                return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCheckoutInfo(UpdateCheckoutInfoVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    message = "Invalid data"
                });
            }

            var result = await accountService.UpdateCheckoutInfo(model);

            return Json(new
            {
                success = result.IsSuccess,
                message = result.IsSuccess
                    ? "Information updated successfully"
                    : result.ErrorMessage
            });
        }
    }
}



