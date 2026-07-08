using System.ComponentModel.DataAnnotations;
using E_commerce.DAL.Entities;
using Microsoft.AspNetCore.Mvc;

namespace E_commerce.BLL.ViewModels
{
    public class ProductDetailsVM
    {
        public Product Product { get; set; } = null!;

        public AddToCartViewModel? AddToCart { get; set; }


    }
    public class AddToCartViewModel
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Remote("ValidateCount", "Product", AdditionalFields = nameof(ProductId))]
        public int Count { get; set; }
    }
}
