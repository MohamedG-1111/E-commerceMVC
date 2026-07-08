using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Pagination;
using Ecommerce.Utility.Result;

namespace BLL.Services.Interfaces
{
    public interface IProductService
    {
        public Task<Result> CreateProductAsync(CreateOrUpdateProductViewModel obj);
        public Task<Result<ProductDetailsVM?>> ProductDetailsAsync(int Id);

        public Task<Result<PaginatedResult<ProductListVm>?>>
            AllProductsAsync(PaginationParameters parameters, string? searchTerm = null, string? category = null);

        public Task<Result> UpdateProductAsync(CreateOrUpdateProductViewModel Obj);
        public Task<Result> DeleteProductAsync(int id);

    }
}
