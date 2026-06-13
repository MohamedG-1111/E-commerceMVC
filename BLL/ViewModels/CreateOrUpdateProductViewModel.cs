using System.ComponentModel.DataAnnotations;
using E_commerce.BLL.Attributes;
using E_commerce.Utility.Settings;
using Microsoft.AspNetCore.Http;

namespace E_commerce.BLL.ViewModels
{
    public class CreateOrUpdateProductViewModel : ProductVm
    {
        [AllowedExtensions(FileSettings.AllowedExtensions)]
        [MaxSizeAllowed(FileSettings.MaxSizeInBytes)]
        [Required]
        public IFormFile Cover { get; set; } = null!;
    }
}
