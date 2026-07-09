using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class AppController : Controller
    {
        protected IActionResult HandleAjaxResult(Result result)
        {
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true
                });
            }
            return result.ErrorType switch
            {
                ErrorType.UNAUTHORIZED => Unauthorized(new
                {
                    message = result.ErrorMessage
                }),

                ErrorType.CONFLICT => Conflict(new
                {
                    message = result.ErrorMessage
                }),

                ErrorType.NOT_FOUND => NotFound(new
                {
                    message = result.ErrorMessage
                }),

                ErrorType.VALIDATION => BadRequest(new
                {
                    message = result.ErrorMessage
                }),

                _ => BadRequest(new
                {
                    message = result.ErrorMessage
                })
            };
        }
        protected IActionResult HandleResult(
         Result result,
         string? viewName = null,
         object? model = null)
        {
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
                return HandleAjaxResult(result);
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
                    return View("Conflict", result.ErrorMessage);
                default:
                    TempData["error"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Index));
            }
        }
    }

}
