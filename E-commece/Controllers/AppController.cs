using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class AppController : Controller
    {
        protected IActionResult HandleResult(
         Result result,
         string? viewName = null,
         object? model = null)
        {
            switch (result.ErrorType)
            {
                case ErrorType.VALIDATION:
                    if (viewName != null)
                    {
                        ModelState.AddModelError(string.Empty, result.ErrorMessage!);
                        return View(viewName, model);
                    }

                    TempData["Error"] = result.ErrorMessage;
                    return RedirectToAction("Index");

                case ErrorType.NOT_FOUND:
                    return View("NotFound", result.ErrorMessage);

                case ErrorType.UNAUTHORIZED:
                    TempData["error"] = result.ErrorMessage;
                    return RedirectToAction("Login", "Auth");
                case ErrorType.CONFLICT:
                    TempData["error"] = result.ErrorMessage;
                    return RedirectToAction("Index", "Cart");
                default:
                    TempData["error"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Index));
            }
        }
    }
}
