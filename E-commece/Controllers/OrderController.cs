using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Ecommerce.Utility;
using Ecommerce.Utility.Pagination;
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
            return RedirectToAction("MyOrders");
        }


        public async Task<IActionResult> MyOrders(PaginationParameters Parameters)
        {
            var result = await orderService.GetMyOrdersAsync(Parameters);
            if (result.IsFailure)
                return HandleResult(result);

            return View(result.Value);
        }

        public async Task<IActionResult> FilterOrders(PaginationParameters Parameters, OrderFilter Filter)
        {
            var result = await orderService.GetMyOrdersAsync(Parameters, Filter);
            if (result.IsFailure)
                return HandleResult(result);

            return PartialView("_MyOrderPartial", result.Value);
        }

        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var result = await orderService.GetOrderDetails(orderId);
            if (result.IsFailure)
                return HandleResult(result);
            return View(result.Value);
        }
    }
}

