using BLL.Services.Interfaces;
using BLL.ViewModels;
using DataAccessLayer.Repositories.Interfaces;
using DataAcessLayer.Models;
using E_commerce.DAL.Entities;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
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

        public async Task<Result<IEnumerable<Category>?>> AllCategoriesAsync()
        {
            var categories = await unitOfWork.Repository<Category>().GetAllAsync();

            var sorted = categories?
                .OrderBy(c => c.DisplayOrder)
                ?? Enumerable.Empty<Category>();

            return Result<IEnumerable<Category>?>.Success(sorted);
        }

        public async Task<Result<CategoryVM?>> CategoryDetailsAsync(int Id)
        {
            if (Id <= 0)
                return Result<CategoryVM?>.Failure("Invalid category Id", "INVALID_CATEGORY_ID", ErrorType.NOT_FOUND);

            var Obj = await unitOfWork.Repository<Category>().FindAsync(Id);
            if (Obj == null)
                return Result<CategoryVM?>.Failure("Category not found.", errorType: ErrorType.NOT_FOUND);


            var categoryVM = new CategoryVM
            {
                Name = Obj.Name,
                DisplayOrder = Obj.DisplayOrder
            };
            return Result<CategoryVM?>.Success(categoryVM);
        }

        public async Task<Result> CreateCategoryAsync(CategoryVM obj)
        {
            if (obj == null)
                return Result.Failure("Invalid category data.", errorType: ErrorType.VALIDATION);
            if (obj.DisplayOrder < 0)
                return Result.Failure("Invalid display order", errorType: ErrorType.VALIDATION);
            var CategoryWithSameName = await unitOfWork.Repository<Category>()
                          .AnyAsync(c => c.Name.ToLower() == obj.Name.Trim().ToLower());
            if (CategoryWithSameName)
                return Result.Failure("A category with the same name already exists.", "CATEGORY_NAME_EXISTS", ErrorType.VALIDATION);
            Category category = new Category
            {
                Name = obj.Name.Trim(),
                DisplayOrder = obj.DisplayOrder
            };


            await unitOfWork.Repository<Category>().AddAsync(category);

            return await unitOfWork.SaveChangesAsync() > 0 ?
                Result.Success() : Result.Failure("Failed to create category.", errorType: ErrorType.INTERNAL_ERROR);
        }

        public async Task<Result> UpdateCategoryAsync(int id, CategoryVM Obj)
        {
            if (id <= 0)
                return Result.Failure("Category not found.", errorType: ErrorType.NOT_FOUND);
            if (Obj is null)
                return Result.Failure("Invalid category data.", errorType: ErrorType.VALIDATION);

            var category = await unitOfWork.Repository<Category>().FindAsync(id);
            if (category == null)
                return Result.Failure("Category not found.", errorType: ErrorType.NOT_FOUND);


            var CategoryWithSameName = await unitOfWork.Repository<Category>()
                          .AnyAsync(c => c.Name.ToLower() == Obj.Name.Trim().ToLower()
                             && c.Id != id);
            if (CategoryWithSameName)
                return Result.Failure("A category with the same name already exists.", "CATEGORY_NAME_EXISTS", ErrorType.VALIDATION);

            if (Obj.DisplayOrder < 0)
                return Result.Failure("Invalid display order", errorType: ErrorType.VALIDATION);

            category.Name = Obj.Name.Trim();
            category.DisplayOrder = Obj.DisplayOrder;
            return await unitOfWork.SaveChangesAsync() > 0 ?
                Result.Success() : Result.Failure("Failed to update category.", errorType: ErrorType.INTERNAL_ERROR);
        }

        public async Task<Result> DeleteCategoryAsync(int id)
        {
            if (id <= 0)
                return Result.Failure("Invalid category Id", "INVALID_CATEGORY_ID", ErrorType.NOT_FOUND);

            var category = await unitOfWork.Repository<Category>().FindAsync(id);

            if (category == null)
                return Result.Failure("Category not found.", errorType: ErrorType.NOT_FOUND);
            var hasProducts = await unitOfWork.Repository<Product>().AnyAsync(p => p.CategoryId == id);
            if (hasProducts)
                return Result.Failure("Cannot delete category because it has associated products.", errorType: ErrorType.VALIDATION);


            unitOfWork.Repository<Category>().Delete(category);

            return await unitOfWork.SaveChangesAsync() > 0 ? Result.Success()
                : Result.Failure("Failed to delete category.", errorType: ErrorType.INTERNAL_ERROR);
        }

        public async Task<IEnumerable<SelectListItem>> GetAllCategoriesItems()
        {
            var categories = await unitOfWork.Repository<Category>().GetAllAsync();
            return categories.Select(c => new SelectListItem
            {
                Text = c.Name.Trim(),
                Value = c.Id.ToString()
            });
        }
    }
}
