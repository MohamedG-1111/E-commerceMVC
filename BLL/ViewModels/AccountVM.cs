using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_commerce.BLL.ViewModels
{
    public class AccountVM
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        public string Phone { get; set; } = string.Empty;

        public string? StreetAddress { get; set; }

        public string? City { get; set; }
        [Required(ErrorMessage = "Postal Code is required")]
        [RegularExpression(@"^\d{5,10}$",
         ErrorMessage = "Invalid Postal Code")]
        public string? PostalCode { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        [ValidateNever]
        public IEnumerable<SelectListItem> Companies { get; set; }

        public int? CompanyId { get; set; }

        [ValidateNever]
        public string? Image { get; set; }
        [ValidateNever]

        public string? CompanyName { get; set; }
    }
}
