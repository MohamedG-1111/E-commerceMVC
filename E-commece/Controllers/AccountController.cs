using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Ecommerce.Utility;
using Ecommerce.Utility.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class AccountController : AppController
    {
        private readonly IAccountService accountService;
        private readonly ICompanyService companyService;


        public AccountController(
            IAccountService accountService,
            ICompanyService companyService)
        {
            this.accountService = accountService;
            this.companyService = companyService;

        }

        #region Public

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var resultEmail = await accountService.GetUserEmail(userId);

            if (resultEmail.IsSuccess)
                ViewBag.Email = resultEmail.Value;

            var result = await accountService.ConfirmEmailAsync(userId, token);

            if (result.IsFailure)
                return HandleResult(result, nameof(ConfirmEmail));

            TempData["Success"] = "Email confirmed successfully!";

            return View();
        }

        [AllowAnonymous]
        public async Task<IActionResult> ResendEmailConfirmation(string email)
        {
            var result = await accountService.ReSendEmailConfirmationAsync(email);

            if (result.IsFailure)
                return HandleResult(result, nameof(ConfirmEmail));

            ViewBag.Email = email;

            TempData["Success"] = "Resend Email Confirmation successfully!";

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await accountService.SendResetPasswordEmailAsync(model.Email);

            if (result.IsFailure)
                return HandleResult(result, nameof(ForgotPassword), model);

            return View("ForgotPasswordConfirmation");
        }

        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await accountService.ResetPasswoAsync(model);

            if (result.IsFailure)
                return HandleResult(result, nameof(ResetPassword), model);

            TempData["Success"] = "Reset Password Successfully";

            return RedirectToAction("Login", "Auth");
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateAccount()
        {
            var model = new AccountVM
            {
                Companies = await companyService.GetAllCompaniesItems()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> CreateAccount(AccountVM model)
        {
            if (!ModelState.IsValid)
            {
                model.Companies = await companyService.GetAllCompaniesItems();
                return View(model);
            }

            var result = await accountService.CreateAccountAsync(model);

            if (result.IsFailure)
            {
                model.Companies = await companyService.GetAllCompaniesItems();
                return HandleResult(result, nameof(CreateAccount), model);
            }

            TempData["Success"] = "Account Created Successfully";

            return RedirectToAction("Login", "Auth");
        }

        #endregion

        #region Admin

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Index(PaginationParameters paramater)
        {
            var result = await accountService.GetAccountsAsync(paramater);

            if (result.IsFailure)
                return HandleResult(result);

            return View(result.Value);
        }

        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> FilterAccounts(PaginationParameters parameter, string search)
        {
            var result = await accountService.GetAccountsAsync(parameter, search);

            if (result.IsFailure)
            {
                return PartialView("_AccountPartial",
                    Enumerable.Empty<AllAccountsViewModel>());
            }

            return PartialView("_AccountPartial", result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> LockAccount(string userId)
        {
            var result = await accountService.LockAccountAsync(userId);

            if (result.IsFailure)
                return Json(new
                {
                    success = false,
                    message = result.ErrorMessage
                });

            return Json(new
            {
                success = true,
                message = "Account locked successfully"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> UnlockAccount(string userId)
        {
            var result = await accountService.UnLockAccountAsync(userId);

            if (result.IsFailure)
                return Json(new
                {
                    success = false,
                    message = result.ErrorMessage
                });

            return Json(new
            {
                success = true,
                message = "Account Unlocked successfully"
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(string userId)
        {
            var result = await accountService.DeleteAccountAsync(userId);

            if (result.IsFailure)
                return HandleResult(result);

            TempData["Success"] = "Account Deleted Successfully";

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Authenticated Users

        [Authorize]
        public async Task<IActionResult> Profile(string userId)
        {
            var result = await accountService.GetAccountByUserId(userId);

            if (result.IsFailure)
                return HandleResult(result);

            ViewBag.UserId = userId;

            return View(result.Value);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit(string userId, string? returnUrl)
        {
            var result = await accountService.GetAccountToEditAsync(userId);

            if (result.IsFailure)
                return HandleResult(result);

            result.Value!.Companies = await companyService.GetAllCompaniesItems();
            ViewBag.ReturnUrl = returnUrl;

            return View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(EditAccountVM model, string? returnUrl)
        {

            if (!ModelState.IsValid)
            {
                model.Companies = await companyService.GetAllCompaniesItems();
                return View(model);
            }


            var result = await accountService.UpdateAccountAsync(model.UserId, model);

            if (result.IsFailure)
            {
                model.Companies = await companyService.GetAllCompaniesItems();
                return HandleResult(result, nameof(Edit), model);
            }

            TempData["Success"] = "Update Successfully";

            if (User.IsInRole(Roles.Admin))
            {
                if (!string.IsNullOrWhiteSpace(returnUrl))
                {
                    return Redirect(returnUrl);

                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Customer},{Roles.Company},{Roles.Employee}")]
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

        #endregion
    }
}