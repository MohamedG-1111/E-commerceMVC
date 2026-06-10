using System.Diagnostics;
using BLL.Services.Interfaces;
using E_commece.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService productService;

        public HomeController(IProductService IProductService)
        {
            this.productService = IProductService;
        }
        public async Task<IActionResult> Index(string item)
        {
            var Products = await productService.AllProductsAsync(item);
            return View(Products);
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
