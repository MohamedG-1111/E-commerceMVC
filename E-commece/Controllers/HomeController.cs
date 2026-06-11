using System.Diagnostics;
using BLL.Services.Interfaces;
using E_commece.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService productService;

        public HomeController(IProductService ProductService)
        {
            this.productService = ProductService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var products = await productService.AllProductsAsync();
            return View(products);
        }
        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await productService.ProductDetailsAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
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
