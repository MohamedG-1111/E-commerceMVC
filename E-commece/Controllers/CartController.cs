using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    [Authorize(Roles = $"{Roles.Customer},{Roles.Employee},{Roles.Company}")]
    public class CartController : AppController
    {
        private readonly ICartService cartService;

        public CartController(ICartService cartService)
        {
            this.cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await cartService.GetAsync();
            if (result.IsFailure)
                return HandleResult(result);
            return View(result.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(AddToCartViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("ProductDetails", "Home", new { id = model.ProductId });

            var result = await cartService.AddProductToCart(model);

            if (result.IsFailure)
                return HandleResult(result);

            return RedirectToAction(nameof(Index), result.Value);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> RemoveProductFromCart(int ProductId)
        {
            var result = await cartService.DeleteProductFromCartAsync(ProductId);
            if (result.IsFailure)
                return HandleResult(result);
            return PartialView("_CartPartial", result.Value);
        }

        [HttpGet]
        public async Task<int> GetCartCount()
        {
            var result = await cartService.GetAsync();

            if (result.IsSuccess && result.Value?.Items != null)
                return result.Value.Items.Sum(x => x.Count);

            return 0;
        }

        public async Task<IActionResult> ChangeQuanatity(int productId, int change)
        {
            var result = await cartService.UpdateQuantityAsync(productId, change);
            if (result.IsFailure)
                return HandleResult(result);
            return PartialView("_CartPartial", result.Value);

        }
        [HttpGet]
        public async Task<IActionResult> ClearCart()
        {
            var result = await cartService.ClearCartAsync();
            if (result.IsFailure)
                return HandleResult(result);
            return PartialView("_CartPartial", result.Value);
        }



    }
}
