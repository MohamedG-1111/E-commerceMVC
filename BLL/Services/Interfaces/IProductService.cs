using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities;
using Ecommerce.Utility.Result;

namespace BLL.Services.Interfaces
{
    public interface IProductService
    {
        public Task<bool> CreateProductAsync(CreateOrUpdateProductViewModel obj);
        public Task<Product?> ProductDetailsAsync(int Id);

        public Task<Result<IEnumerable<ProductListVm>?>> AllProductsAsync(string? searchTerm = null, string? category = null);

        public Task<bool> UpdateProductAsync(CreateOrUpdateProductViewModel Obj);
        public Task<bool> DeleteProductAsync(int id);

    }
}
