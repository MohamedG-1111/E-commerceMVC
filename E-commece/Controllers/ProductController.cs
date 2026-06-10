using BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(string? searchTerm)
        {
            var products = await _productService.AllProductsAsync(searchTerm);
            return View(products);
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            CreateOrUpdateProductViewModel model = new()
            {
                Categories = await _categoryService.GetAllCategoriesItems()
            };
            return View("UpSert", model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrUpdateProductViewModel model)
        {
            ModelState.Remove("Product.Id");

            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllCategoriesItems();
                return View("UpSert", model);
            }
            var result = await _productService.CreateProductAsync(model);
            if (result)
                TempData["Success"] = "Product Created Successfully";
            else
                TempData["Error"] = "Failed to Create Product";
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.ProductDetailsAsync(id);
            if (product == null)
                return NotFound();

            var model = new CreateOrUpdateProductViewModel
            {
                Product = product,
                Categories = await _categoryService.GetAllCategoriesItems()
            };

            return View("UpSert", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateOrUpdateProductViewModel model)
        {
            if (id != model.Product.Id)
                return BadRequest();

            ModelState.Remove("Cover"); // Remove validation for Cover if not provided during edit
            if (!ModelState.IsValid)
            {
                model.Categories = await _categoryService.GetAllCategoriesItems();
                return View("UpSert", model);
            }

            var result = await _productService.UpdateProductAsync(model);
            if (result)
                TempData["Success"] = "Product Updated Successfully";
            else
                TempData["Error"] = "Failed to Update Product";
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.ProductDetailsAsync(id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (result)
                TempData["Success"] = "Product Deleted Successfully";
            else
                TempData["Error"] = "Failed to Delete Product";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.ProductDetailsAsync(id);
            if (product == null)
                return NotFound();
            return View(product);

        }
        [HttpGet]
        [Route("Product/SearchAsync")]
        public async Task<IActionResult> SearchAsync(string searchTerm)
        {
            var products = await _productService.AllProductsAsync(searchTerm);
            return PartialView("_BooksPartial", products);
        }
    }
}
