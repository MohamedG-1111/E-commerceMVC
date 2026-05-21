using E_commece.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commece.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Electronics", DisplayOrder = 1 },
                new Category { Id = 2, Name = "Clothes", DisplayOrder = 2 },
                new Category { Id = 3, Name = "Food", DisplayOrder = 3 }
            );
        }
        public DbSet<Category> Categories { get; set; }
    }
}
