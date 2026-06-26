using System.ComponentModel.DataAnnotations;

namespace E_commerce.BLL.ViewModels
{
    public class UpdateCheckoutInfoVM
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null;
        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null;
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;
        [Required(ErrorMessage = "Street Address is required")]
        [StringLength(200)]
        public string StreetAddress { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postal Code is required")]
        [StringLength(10)]
        public string PostalCode { get; set; } = string.Empty;
    }
}
