using E_commerce.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        private readonly ICartService cartService;

        public CartCountViewComponent(ICartService cartService)
        {
            this.cartService = cartService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var result = await cartService.GetAsync();

            int count = 0;

            if (result.IsSuccess && result.Value?.Items != null)
            {
                count = result.Value.Items.Sum(x => x.Count);
            }

            return View(count);

        }
    }
}
