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
                new Category { Id = 1, Name = "Programming", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Novels", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Science", DisplayOrder = 3 }
            );

            // =========================
            // Books (Products) Seed
            // =========================
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "Clean Code",
                    Description = "A Handbook of Agile Software Craftsmanship",
                    ISBN = "9780132350884",
                    ListPrice = 500,
                    PriceFor1To50 = 450,
                    PriceFor50Plus = 420,
                    PriceFor100Plus = 400,
                    ImageUrl = "",
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Title = "The Pragmatic Programmer",
                    Description = "Your Journey to Mastery",
                    ISBN = "9780201616224",
                    ListPrice = 550,
                    PriceFor1To50 = 500,
                    PriceFor50Plus = 470,
                    PriceFor100Plus = 450,
                    ImageUrl = "",
                    CategoryId = 1
                },
                new Product
                {
                    Id = 3,
                    Title = "Design Patterns",
                    Description = "Elements of Reusable Object-Oriented Software",
                    ISBN = "9780201633610",
                    ListPrice = 600,
                    PriceFor1To50 = 550,
                    PriceFor50Plus = 520,
                    PriceFor100Plus = 500,
                    ImageUrl = "",
                    CategoryId = 1
                },
                new Product
                {
                    Id = 4,
                    Title = "Harry Potter",
                    Description = "Fantasy novel series",
                    ISBN = "9780747532743",
                    ListPrice = 300,
                    PriceFor1To50 = 270,
                    PriceFor50Plus = 250,
                    PriceFor100Plus = 230,
                    ImageUrl = "",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 5,
                    Title = "The Hobbit",
                    Description = "Fantasy adventure novel",
                    ISBN = "9780261103344",
                    ListPrice = 320,
                    PriceFor1To50 = 290,
                    PriceFor50Plus = 270,
                    PriceFor100Plus = 250,
                    ImageUrl = "",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 6,
                    Title = "1984",
                    Description = "Dystopian novel",
                    ISBN = "9780451524935",
                    ListPrice = 280,
                    PriceFor1To50 = 250,
                    PriceFor50Plus = 230,
                    PriceFor100Plus = 210,
                    ImageUrl = "",
                    CategoryId = 2
                },
                new Product
                {
                    Id = 7,
                    Title = "A Brief History of Time",
                    Description = "Cosmology explained",
                    ISBN = "9780553380163",
                    ListPrice = 400,
                    PriceFor1To50 = 370,
                    PriceFor50Plus = 350,
                    PriceFor100Plus = 330,
                    ImageUrl = "",
                    CategoryId = 3
                },
                new Product
                {
                    Id = 8,
                    Title = "The Selfish Gene",
                    Description = "Evolutionary biology book",
                    ISBN = "9780199291151",
                    ListPrice = 420,
                    PriceFor1To50 = 390,
                    PriceFor50Plus = 370,
                    PriceFor100Plus = 350,
                    ImageUrl = "",
                    CategoryId = 3
                },
                new Product
                {
                    Id = 9,
                    Title = "Cosmos",
                    Description = "Science and universe exploration",
                    ISBN = "9780345539434",
                    ListPrice = 450,
                    PriceFor1To50 = 420,
                    PriceFor50Plus = 400,
                    PriceFor100Plus = 380,
                    ImageUrl = "",
                    CategoryId = 3
                },
                new Product
                {
                    Id = 10,
                    Title = "Deep Work",
                    Description = "Rules for focused success",
                    ISBN = "9781455586691",
                    ListPrice = 380,
                    PriceFor1To50 = 350,
                    PriceFor50Plus = 330,
                    PriceFor100Plus = 310,
                    ImageUrl = "",
                    CategoryId = 1
                }
            );
        }
        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }
    }
}
