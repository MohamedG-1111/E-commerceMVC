using E_commerce.BLL.ViewModels;
using Ecommerce.Utility.Result;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_commerce.BLL.Services.Interfaces
{
    public interface ICompanyService
    {
        public Task<Result> CreateCompanyAsync(CompanyInfoVM obj);
        public Task<Result<CompanyInfoVM?>> CompanyDetailsAsync(int Id);

        public Task<Result<IEnumerable<CompanyVM>?>> AllCompaniesAsync();

        public Task<Result> UpdateCompanyAsync(int id, CompanyInfoVM Obj);
        public Task<Result> DeleteCompanyAsync(int id);

        public Task<IEnumerable<SelectListItem>> GetAllCategoriesItems();

        public Task<Result<IEnumerable<CompanyVM>?>> SearchAsync(string? search);
    }
}
