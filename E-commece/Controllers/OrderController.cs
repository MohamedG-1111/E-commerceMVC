using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class OrderController : AppController
    {
        public IActionResult CheckOut()
        {
            return View();
        }
    }
}
