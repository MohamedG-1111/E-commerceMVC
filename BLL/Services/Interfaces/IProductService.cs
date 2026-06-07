using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities;

namespace BLL.Services.Interfaces
{
    public interface IProductService
    {
        public Task<bool> CreateProductAsync(CreateProductViewModel obj);
        public Task<Product?> ProductDetailsAsync(int Id);

        public Task<IEnumerable<ProductListVm>?> AllProductsAsync();

        public Task<bool> UpdateProductAsync(int id, Product Obj);
        public Task<bool> DeleteProductAsync(int id);

    }
}
