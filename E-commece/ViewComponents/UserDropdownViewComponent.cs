using E_commerce.BLL.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.ViewComponents
{
    public class UserDropdownViewComponent : ViewComponent
    {
        private readonly ICurrentUserService currentUserService;

        public UserDropdownViewComponent(ICurrentUserService currentUserService)
        {
            this.currentUserService = currentUserService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await currentUserService.GetCurrentUser();
            return View(currentUser);
        }
    }
}
