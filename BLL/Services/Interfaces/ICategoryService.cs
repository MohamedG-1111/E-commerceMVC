using BLL.ViewModels;
using DataAcessLayer.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BLL.Services.Interfaces
{
    public interface ICategoryService
    {
        public Task<bool> CreateCategoryAsync(CategoryVM obj);
        public Task<CategoryVM?> CategoryDetailsAsync(int Id);

        public Task<IEnumerable<Category>?> AllCategoriesAsync();

        public Task<bool> UpdateCategoryAsync(int id, CategoryVM Obj);
        public Task<bool> DeleteCategoryAsync(int id);

        public Task<IEnumerable<SelectListItem>> GetAllCategoriesItems();

    }
}
