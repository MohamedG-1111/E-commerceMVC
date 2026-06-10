using DataAccessLayer.Repositories.Implementation;
using DataAcessLayer.Data;
using E_commerce.DAL.Entities;
using E_commerce.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.DAL.Repositories.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }
        public IQueryable<Product> Search(string searchTerm)
        {
            var query = context.Products
                    .Include(p => p.Category);

            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            return query.Where(p =>
                p.Title.Contains(searchTerm) ||
                p.Author.Contains(searchTerm) ||
                p.ISBN.Contains(searchTerm));
        }
    }
}
