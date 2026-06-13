using BLL.Services.Interfaces;
using BLL.ViewModels;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService CategoryService;

        public CategoryController(ICategoryService _CategoryService)
        {
            CategoryService = _CategoryService;
        }
        public async Task<IActionResult> Index()
        {
            var result = await CategoryService.AllCategoriesAsync();
            return View(result.Value);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(CategoryVM obj)
        {
            if (!ModelState.IsValid)
                return View(obj);
            var result = await CategoryService.CreateCategoryAsync(obj);
            if (result.IsFailure)
                return HandleResult(result, nameof(Create), obj);
            TempData["success"] = "Category Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await CategoryService.CategoryDetailsAsync(id);
            if (result.IsFailure)
                return HandleResult(result);
            return View(result.Value);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, CategoryVM obj)
        {
            if (!ModelState.IsValid)
                return View(obj);
            var result = await CategoryService.UpdateCategoryAsync(id, obj);
            if (result.IsFailure)
                return HandleResult(result, nameof(Edit), obj);
            TempData["success"] = "Category Updated Successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmDelete(int id)
        {

            var result = await CategoryService.DeleteCategoryAsync(id);
            if (result.IsFailure)
            {
                if (result.ErrorType == ErrorType.VALIDATION)
                {
                    TempData["error"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Index));
                }
                return HandleResult(result);
            }
            TempData["success"] = "Category Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var Result = await CategoryService.CategoryDetailsAsync(id);
            if (Result.IsFailure)
                return HandleResult(Result);
            return View(Result.Value);

        }

        private IActionResult HandleResult(
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
