using System.Diagnostics;
using BLL.Services.Interfaces;
using E_commece.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class HomeController : AppController
    {
        private readonly IProductService productService;

        public HomeController(IProductService ProductService)
        {
            this.productService = ProductService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var Resultproducts = await productService.AllProductsAsync();
            return View(Resultproducts.Value);
        }
        public async Task<IActionResult> ProductDetails(int id)
        {
            var Resultproduct = await productService.ProductDetailsAsync(id);
            if (!Resultproduct.IsSuccess)
                return HandleResult(Resultproduct);

            return View(Resultproduct.Value);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
