using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAcessLayer.Models;

namespace E_commerce.DAL.Entities
{
    public class Product
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;

        [Display(Name = "List Price")]
        [Range(1, 1000, ErrorMessage = "List Price must be between 1 and 1000.")]
        public int ListPrice { get; set; }


        [Display(Name = "Price for 1-50")]
        public int PriceFor1To50 { get; set; }


        [Display(Name = "Price for 50+")]
        public int PriceFor50Plus { get; set; }

        [Display(Name = "Price for 100+")]
        public int PriceFor100Plus { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

    }
}

