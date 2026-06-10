using DataAcessLayer.Models;
using E_commerce.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAcessLayer.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =========================
            // Categories Seed
            // =========================
            modelBuilder.Entity<Category>().HasData(
     new Category
     {
         Id = 1,
         Name = "Programming",
         DisplayOrder = 1
     },
     new Category
     {
         Id = 2,
         Name = "Database",
         DisplayOrder = 2
     },
     new Category
     {
         Id = 3,
         Name = "Web Development",
         DisplayOrder = 3
     },
     new Category
     {
         Id = 4,
         Name = "Software Engineering",
         DisplayOrder = 4
     },
     new Category
     {
         Id = 5,
         Name = "Artificial Intelligence",
         DisplayOrder = 5
     }
 );

            // =========================
            // Books (Products) Seed
            // =========================
            modelBuilder.Entity<Product>().HasData(

                new Product
                {
                    Id = 1,
                    Title = "Clean Code",
                    Author = "Robert Martin",
                    Description = "A handbook of agile software craftsmanship.",
                    ISBN = "9780132350884",
                    ListPrice = 60,
                    PriceFor1To50 = 55,
                    PriceFor50Plus = 50,
                    PriceFor100Plus = 45,
                    CategoryId = 4,
                },

                new Product
                {
                    Id = 2,
                    Title = "The Pragmatic Programmer",
                    Author = "Andrew Hunt",
                    Description = "Classic guide for software developers.",
                    ISBN = "9780135957059",
                    ListPrice = 65,
                    PriceFor1To50 = 60,
                    PriceFor50Plus = 55,
                    PriceFor100Plus = 50,
                    CategoryId = 4,
                },

                new Product
                {
                    Id = 3,
                    Title = "Design Patterns",
                    Author = "Erich Gamma",
                    Description = "Elements of reusable object oriented software.",
                    ISBN = "9780201633610",
                    ListPrice = 70,
                    PriceFor1To50 = 65,
                    PriceFor50Plus = 60,
                    PriceFor100Plus = 55,
                    CategoryId = 4,
                },

                new Product
                {
                    Id = 4,
                    Title = "C# in Depth",
                    Author = "Jon Skeet",
                    Description = "Deep dive into advanced C# concepts.",
                    ISBN = "9781617294532",
                    ListPrice = 55,
                    PriceFor1To50 = 50,
                    PriceFor50Plus = 45,
                    PriceFor100Plus = 40,
                    CategoryId = 1,
                },

                new Product
                {
                    Id = 5,
                    Title = "ASP.NET Core",
                    Author = "Adam Freeman",
                    Description = "Comprehensive guide to ASP.NET Core.",
                    ISBN = "9781484269237",
                    ListPrice = 75,
                    PriceFor1To50 = 70,
                    PriceFor50Plus = 65,
                    PriceFor100Plus = 60,
                    CategoryId = 3,
                },

                new Product
                {
                    Id = 6,
                    Title = "SQL Server",
                    Author = "Itzik BenGan",
                    Description = "Practical T SQL fundamentals and techniques.",
                    ISBN = "9781509302000",
                    ListPrice = 50,
                    PriceFor1To50 = 45,
                    PriceFor50Plus = 40,
                    PriceFor100Plus = 35,
                    CategoryId = 2,
                },

                new Product
                {
                    Id = 7,
                    Title = "Python Crash",
                    Author = "Eric Matthes",
                    Description = "Fast paced introduction to Python.",
                    ISBN = "9781718502703",
                    ListPrice = 45,
                    PriceFor1To50 = 40,
                    PriceFor50Plus = 35,
                    PriceFor100Plus = 30,
                    CategoryId = 1,
                },

                new Product
                {
                    Id = 8,
                    Title = "Deep Learning",
                    Author = "Ian Goodfellow",
                    Description = "Foundational deep learning concepts.",
                    ISBN = "9780262035613",
                    ListPrice = 80,
                    PriceFor1To50 = 75,
                    PriceFor50Plus = 70,
                    PriceFor100Plus = 65,
                    CategoryId = 5,
                },

                new Product
                {
                    Id = 9,
                    Title = "Hands On ML",
                    Author = "Aurelien Geron",
                    Description = "Machine learning with Scikit Learn and TensorFlow.",
                    ISBN = "9781098125974",
                    ListPrice = 85,
                    PriceFor1To50 = 80,
                    PriceFor50Plus = 75,
                    PriceFor100Plus = 70,
                    CategoryId = 5,
                },

                new Product
                {
                    Id = 10,
                    Title = "JavaScript Guide",
                    Author = "David Flanagan",
                    Description = "Definitive JavaScript reference and guide.",
                    ISBN = "9781491952023",
                    ListPrice = 60,
                    PriceFor1To50 = 55,
                    PriceFor50Plus = 50,
                    PriceFor100Plus = 45,
                    CategoryId = 3,
                }
            );
        }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}
