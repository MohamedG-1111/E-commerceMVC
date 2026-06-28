using BLL.ViewModels;
using DataAcessLayer.Models;
using Ecommerce.Utility.Pagination;
using Ecommerce.Utility.Result;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task<Result> CreateCategoryAsync(CategoryVM obj);
        public Task<Result<CategoryVM?>> CategoryDetailsAsync(int Id);
        public Task<Result> UpdateCategoryAsync(int id, CategoryVM Obj);
        public Task<Result> DeleteCategoryAsync(int id);

        public Task<IEnumerable<SelectListItem>> GetAllCategoriesItems();

        public Task<Result<PaginatedResult<Category>>> AllCategoriesAsync(PaginationParameters parameters, string? searchItem = null);

    }
}
