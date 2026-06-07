using BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _productService.AllProductsAsync();
            return View(products);
        }
    }
}
