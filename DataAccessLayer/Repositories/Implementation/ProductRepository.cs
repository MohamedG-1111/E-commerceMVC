using DataAccessLayer.Repositories.Implementation;
using DataAcessLayer.Data;
using E_commerce.DAL.Entities;
using E_commerce.DAL.Repositories.Interfaces;

namespace E_commerce.DAL.Repositories.Implementation
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly AppDbContext context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }
        public IQueryable<Product> Search(string? searchTerm, string? category)
        {
            var query = context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {

                query = query.Where(p =>
                    p.Title.ToLower().Contains(searchTerm) ||
                    p.Author.ToLower().Contains(searchTerm) ||
                    p.ISBN.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {

                query = query.Where(p =>
                    p.Category != null &&
                    p.Category.Name == category);
            }

            return query;
        }
    }
}
