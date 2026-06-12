using BLL.Services.Interfaces;
using BLL.ViewModels;
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
            var categories = await CategoryService.AllCategoriesAsync();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryVM obj)
        {
            if (!ModelState.IsValid)
                return View(obj);
            var Result = await CategoryService.CreateCategoryAsync(obj);
            if (Result.IsSuccess)
            {
                TempData["success"] = "Category created successfully";
            }
            else
            {
                TempData["error"] = Result.ErrorMessage;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var categoryVM = await CategoryService.CategoryDetailsAsync(id);
            return View(categoryVM);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryVM obj)
        {
            var IsUpdated = await CategoryService.UpdateCategoryAsync(id, obj);
            if (IsUpdated)
            {
                TempData["success"] = "Category updated successfully";
            }
            else
            {
                TempData["error"] = "Failed to update category";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var categoryVM = await CategoryService.CategoryDetailsAsync(id);
            if (categoryVM == null)
                return NotFound();
            ViewBag.id = id;
            return View(categoryVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ConfirmDelete(int id)
        {

            var isDeleted = await CategoryService.DeleteCategoryAsync(id);
            if (isDeleted)
            {
                TempData["success"] = "Category deleted successfully";
            }
            else
            {
                TempData["error"] = "Failed to delete category";
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            var category = await CategoryService.CategoryDetailsAsync(id);
            if (category == null)
                return NotFound();
            return View(category);
        }
    }
}
