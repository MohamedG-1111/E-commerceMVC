using DataAccessLayer.Repositories.Interfaces;
using E_commerce.DAL.Entities;

namespace E_commerce.DAL.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        public IQueryable<Product> Search(string searchTerm);
    }
}
