using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Ecommerce.Utility;
using Ecommerce.Utility.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{

    public class OrderController : AppController
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        [HttpGet]
        [Authorize(Roles = $"{Roles.Customer},{Roles.Company}")]
        public async Task<IActionResult> CheckOut()
        {
            var result = await orderService.GetCheckoutData();
            if (result.IsFailure)
                return HandleResult(result);
            return View(result.Value);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Roles.Customer},{Roles.Company}")]
        public async Task<IActionResult> PlaceOrder()
        {
            var result = await orderService.PlaceOrderAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")

                return HandleAjaxResult(result);

            if (result.IsSuccess)
            {
                TempData["success"] = "Order placed successfully.";
                return RedirectToAction(nameof(AllOrders));
            }

            return HandleResult(result);
        }





        [Authorize(Roles = $"{Roles.Customer},{Roles.Company},{Roles.Admin},{Roles.Employee}")]
        public async Task<IActionResult> AllOrders(PaginationParameters Parameters)
        {
            var result = await orderService.GetOrdersAsync(Parameters);
            if (result.IsFailure)
                return HandleResult(result);

            return View(result.Value);
        }
        [Authorize(Roles = $"{Roles.Customer},{Roles.Company},{Roles.Admin},{Roles.Employee}")]
        public async Task<IActionResult> FilterOrders(PaginationParameters Parameters, OrderFilter Filter)
        {
            var result = await orderService.GetOrdersAsync(Parameters, Filter);
            if (result.IsFailure)
                return HandleResult(result);

            return PartialView("_MyOrderPartial", result.Value);
        }
        [Authorize(Roles = $"{Roles.Customer},{Roles.Company}")]
        public async Task<IActionResult> GetOrderDetails(int orderId)
        {
            var result = await orderService.GetOrderDetails(orderId);
            if (result.IsFailure)
                return HandleResult(result);
            return View(result.Value);
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]

        public async Task<IActionResult> GetOrderDetailsForAdmin(int orderId)
        {
            var result = await orderService.GetOrderDetailsForAdmin(orderId);
            if (result.IsFailure)
                return HandleResult(result);
            return View(result.Value);
        }

        [Authorize(Roles = $"{Roles.Admin},{Roles.Employee}")]
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UpdateStatus(UpdateOrderStatus updateOrderStatus)
        {
            var result = await orderService.UpdateOrderStatus(updateOrderStatus);
            if (result.IsFailure)
                return HandleResult(result);
            TempData["Success"] = "Order status updated successfully.";
            return RedirectToAction(nameof(GetOrderDetailsForAdmin), new { orderId = updateOrderStatus.Id });
        }
    }
}

