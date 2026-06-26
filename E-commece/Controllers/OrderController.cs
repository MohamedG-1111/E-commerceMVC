using E_commerce.BLL.Services.Interfaces;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    [Authorize(Roles = $"{Roles.Customer},{Roles.Employee},{Roles.Company}")]
    public class OrderController : AppController
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            var result = await orderService.GetCheckoutData();
            if (result.IsFailure)
                return HandleResult(result);
            return View(result.Value);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder()
        {
            var result = await orderService.PlaceOrderAsync();
            if (result.IsFailure)
            {
                TempData["Error"] = result.ErrorMessage;
                return RedirectToAction(nameof(CheckOut));
            }

            TempData["Success"] = "Order placed successfully.";
            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> MyOrders()
        {
            var result = await orderService.GetMyOrdersAsync();
            if (result.IsFailure)
                return HandleResult(result);

            return View(result.Value);
        }
    }
}
