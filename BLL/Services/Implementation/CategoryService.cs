using BLL.Services.Interfaces;
using BLL.ViewModels;
using DataAccessLayer.Repositories.Interfaces;
using DataAcessLayer.Models;
using Ecommerce.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BLL.Services.Implementation
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork unitOfWork;

        public CategoryService(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
        }

        public async Task<IEnumerable<Category>?> AllCategoriesAsync()
        {
            return await unitOfWork.Repository<Category>().GetAllAsync();

        }

        public async Task<CategoryVM?> CategoryDetailsAsync(int Id)
        {
            var Obj = await unitOfWork.Repository<Category>().FindAsync(Id);
            if (Obj == null)
                return null;

            return new CategoryVM
            {
                Name = Obj.Name,
                DisplayOrder = Obj.DisplayOrder
            };
        }

        public async Task<Result> CreateCategoryAsync(CategoryVM obj)
        {
            if (obj == null)
                return Result.Failure("Invalid category data.");
            Category category = new Category
            {
                Name = obj.Name,
                DisplayOrder = obj.DisplayOrder
            };
            var existingCategory = await unitOfWork.Repository<Category>().AnyAsync(c => c.Name.ToLower() == obj.Name.ToLower());

            if (existingCategory)
                return Result.Failure("A category with the same name already exists.");

            await unitOfWork.Repository<Category>().AddAsync(category);

            return await unitOfWork.SaveChangesAsync() > 0 ?
                Result.Success() : Result.Failure("Failed to create category.");
        }

        public async Task<bool> UpdateCategoryAsync(int id, CategoryVM Obj)
        {
            if (id <= 0 || Obj == null)
                return false;
            var category = await unitOfWork.Repository<Category>().FindAsync(id);
            if (category == null)
                return false;
            category.Name = Obj.Name;
            category.DisplayOrder = Obj.DisplayOrder;
            return await unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            if (id <= 0)
                return false;

            var category = await unitOfWork.Repository<Category>().FindAsync(id);

            if (category == null)
                return false;

            unitOfWork.Repository<Category>().Delete(category);

            return await unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<SelectListItem>> GetAllCategoriesItems()
        {
            var categories = await unitOfWork.Repository<Category>().GetAllAsync();
            return categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
        }
    }
}
