using System.ComponentModel.DataAnnotations;

namespace E_commerce.BLL.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email is required")]

        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]

        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}
