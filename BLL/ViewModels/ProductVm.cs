using E_commerce.DAL.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace E_commerce.BLL.ViewModels
{
    public class ProductVm
    {
        public Product Product { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
