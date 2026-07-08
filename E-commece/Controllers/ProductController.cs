using BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Ecommerce.Utility;
using Ecommerce.Utility.Pagination;
using Ecommerce.Utility.ResultPattern;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Index(PaginationParameters parameters)
        {
            var Resultproducts = await _productService.AllProductsAsync(parameters);
            if (Resultproducts.IsFailure)
                return HandleResult(Resultproducts);

            return View(Resultproducts.Value);
        }
        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
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
        [Authorize(Roles = Roles.Admin)]
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
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id, string? returnUrl)
        {
            var Resultproduct = await _productService.ProductDetailsAsync(id);
            if (Resultproduct.IsFailure)
                return HandleResult(Resultproduct);

            var model = new CreateOrUpdateProductViewModel
            {
                Product = Resultproduct.Value!.Product,
                Categories = await _categoryService.GetAllCategoriesItems()
            };
            ViewBag.ReturnUrl = returnUrl;
            return View("UpSert", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Edit(int id, CreateOrUpdateProductViewModel model, string? returnUrl)
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
            return Redirect(returnUrl!);
        }

        [HttpGet]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> Delete(int id)
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

        [Authorize(Roles = Roles.Admin)]
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
        [AllowAnonymous]
        public async Task<IActionResult> Search(PaginationParameters parameter, string? searchTerm, string? category)
        {
            var Resultproducts = await _productService.AllProductsAsync(parameter, searchTerm, category);
            return PartialView("~/Views/Product/_BooksPartial.cshtml", Resultproducts.Value);
        }

        [HttpGet]
        [Route("Product/SearchTable")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<IActionResult> SearchTable(PaginationParameters parameter, string? searchTerm, string? category)
        {
            var result = await _productService.AllProductsAsync(parameter, searchTerm, category);

            return PartialView("~/Views/Product/_ProductTablePartial.cshtml", result.Value);
        }

        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> ValidateCount(int count, int productId)
        {
            var result = await _productService.ProductDetailsAsync(productId);

            if (result.IsFailure)
                return Json("Product not found.");

            var product = result.Value!.Product;

            if (count < 1)
                return Json("Quantity must be greater than 0.");

            if (count > product.Stock)
                return Json("The requested quantity is not available.");
            return Json(true);
        }
    }
}

