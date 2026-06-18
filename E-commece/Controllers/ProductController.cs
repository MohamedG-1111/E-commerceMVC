using BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class ProductController : AppController
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Resultproducts = await _productService.AllProductsAsync();
            return View(Resultproducts.Value);
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
            if (result.IsFailure)
                return HandleResult(result, nameof(Create), model);
            TempData["Success"] = "Product Created Successfully ";
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var Resultproduct = await _productService.ProductDetailsAsync(id);
            if (Resultproduct.IsFailure)
                return HandleResult(Resultproduct);

            var model = new CreateOrUpdateProductViewModel
            {
                Product = Resultproduct.Value,
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
            if (result.IsFailure)
            {
                if (result.ErrorType == ErrorType.VALIDATION)
                {
                    return HandleResult(result, nameof(Edit), model);

                }
                return HandleResult(result);
            }
            else
                TempData["Success"] = "Product Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (result.IsFailure)
            {
                if (result.ErrorType == ErrorType.VALIDATION)
                {
                    TempData["error"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Index));
                }
                return HandleResult(result);
            }
            else
                TempData["Success"] = "Product Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Details(int id)
        {
            var Resultproduct = await _productService.ProductDetailsAsync(id);
            if (Resultproduct.IsFailure)
            {
                return HandleResult(Resultproduct);
            }
            return View(Resultproduct.Value);


        }
        [HttpGet]
        [Route("Product/Search")]
        public async Task<IActionResult> Search(string? searchTerm, string? category)
        {
            var Resultproducts = await _productService.AllProductsAsync(searchTerm, category);
            return PartialView("~/Views/Product/_BooksPartial.cshtml", Resultproducts.Value);
        }

        [HttpGet]
        [Route("Product/SearchTable")]
        public async Task<IActionResult> SearchTable(string? searchTerm, string? category)
        {
            var result = await _productService.AllProductsAsync(searchTerm, category);

            return PartialView("~/Views/Product/_ProductTablePartial.cshtml", result.Value);
        }
    }
}

