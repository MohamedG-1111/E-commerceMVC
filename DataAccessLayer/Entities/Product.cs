using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAcessLayer.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace E_commerce.DAL.Entities
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string Title { get; set; } = string.Empty;
        [Required]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 20 characters.")]
        public string Author { get; set; } = string.Empty;
        [Required]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 200 characters.")]

        public string Description { get; set; } = string.Empty;
        [Required]
        public string ISBN { get; set; } = string.Empty;

        [Display(Name = "List Price")]
        [Required]
        [Range(1, 1000, ErrorMessage = "List Price must be between 1 and 1000.")]
        public int ListPrice { get; set; }
        [Required]

        [Display(Name = "Price for 1-50")]
        public int PriceFor1To50 { get; set; }
        [Required]

        [Display(Name = "Price for 50+")]
        public int PriceFor50Plus { get; set; }
        [Required]
        [Display(Name = "Price for 100+")]
        public int PriceFor100Plus { get; set; }
        [Required]
        public int Stock { get; set; }


        public string ImageUrl { get; set; } = string.Empty;

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ValidateNever]

        public Category Category { get; set; }

    }
}

