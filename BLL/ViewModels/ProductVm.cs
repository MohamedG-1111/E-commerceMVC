using E_commerce.DAL.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace E_commerce.BLL.ViewModels
{
    public class ProductVm
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
