using BLL.Services.Interfaces;
using Ecommerce.Utility.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    [AllowAnonymous]
    public class HomeController : AppController
    {
        private readonly IProductService productService;

        public HomeController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(PaginationParameters parameter)
        {
            var result = await productService.AllProductsAsync(parameter);

            if (result.IsFailure)
                return HandleResult(result);

            return View(result.Value);
        }
        //[HttpGet]
        //public async Task<IActionResult> Index(PaginationParameters parameter)
        //{
        //    var result = await productService.AllProductsAsync(parameter);

        //    return Content("Service Works");
        //}
        [HttpGet]
        public async Task<IActionResult> ProductDetails(int id)
        {
            var result = await productService.ProductDetailsAsync(id);

            if (result.IsFailure)
                return HandleResult(result);

            return View(result.Value);
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Error()
        {
            return View();
        }
    }
}