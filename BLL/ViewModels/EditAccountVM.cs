using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_commerce.BLL.ViewModels
{
    public class EditAccountVM
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

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

        [StringLength(10)]
        public string? PostalCode { get; set; }

        public IFormFile? ProfilePicture { get; set; }

        public int? CompanyId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? Companies { get; set; }

        // optional display only
        public string? ExistingImage { get; set; }


        public string? Role { get; set; }

    }
}
