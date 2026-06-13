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
                    ModelState.AddModelError("", result.ErrorMessage!);
                    return View(viewName, model);

                case ErrorType.NOT_FOUND:
                    return View("NotFound", result.ErrorMessage);

                default:
                    TempData["error"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Index));
            }
        }
    }
}
