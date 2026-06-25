using System.ComponentModel.DataAnnotations;

namespace E_commerce.BLL.ViewModels
{
    public class CheckoutViewModel
    {
        public string FullName { get; set; } = null;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = string.Empty;

        [StringLength(10)]
        public string? PostalCode { get; set; }
        public List<CartItem> Items { get; set; } = new();

        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
    }
}
