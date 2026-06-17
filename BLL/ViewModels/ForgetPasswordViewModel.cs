using System.ComponentModel.DataAnnotations;

namespace E_commerce.BLL.ViewModels
{
    public class ForgetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
