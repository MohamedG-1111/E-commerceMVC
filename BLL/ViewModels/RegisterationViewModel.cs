using System.ComponentModel.DataAnnotations;
using E_commerce.BLL.Attributes;
using Microsoft.AspNetCore.Http;

namespace E_commerce.BLL.ViewModels
{
    public class RegisterationViewModel
    {

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(10, MinimumLength = 2,
        ErrorMessage = "First Name must be between 2 and 10 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(10, MinimumLength = 2,
            ErrorMessage = "Last Name must be between 2 and 10 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Street Address is required")]
        [StringLength(100)]
        public string? StreetAddress { get; set; }

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string? City { get; set; }

        [Required(ErrorMessage = "Postal Code is required")]
        [RegularExpression(@"^\d{5,10}$",
            ErrorMessage = "Invalid Postal Code")]
        public string? PostalCode { get; set; }

        [Required(ErrorMessage = "Email is required")]

        [DataType(DataType.EmailAddress, ErrorMessage = "Invalid Email Address")]
        [UniqueEmail]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain uppercase, lowercase, number and special character")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm Password is required")]
        [DataType(DataType.Password)]
        [Compare("Password",
            ErrorMessage = "Password and Confirm Password do not match")]
        public string ConfirmPassword { get; set; } = string.Empty;


        public IFormFile? ProfilePicture { get; set; }
    }
}

