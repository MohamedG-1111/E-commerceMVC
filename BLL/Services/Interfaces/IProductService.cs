using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities;

namespace BLL.Services.Interfaces
{
    public interface IProductService
    {
        public Task<bool> CreateProductAsync(CreateOrUpdateProductViewModel obj);
        public Task<Product?> ProductDetailsAsync(int Id);

        public Task<IEnumerable<ProductListVm>?> AllProductsAsync(string? searchTerm = "", string? category = "");

        public Task<bool> UpdateProductAsync(CreateOrUpdateProductViewModel Obj);
        public Task<bool> DeleteProductAsync(int id);

    }
}
