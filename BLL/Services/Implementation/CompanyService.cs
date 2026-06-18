using DataAccessLayer.Repositories.Interfaces;
using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using E_commerce.DAL.Entities.Users;
using Ecommerce.Utility.Result;
using Ecommerce.Utility.ResultPattern;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace E_commerce.BLL.Services.Implementation
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork unitOfWork;

        public CompanyService(IUnitOfWork UnitOfWork)
        {
            unitOfWork = UnitOfWork;
        }
        public async Task<Result<IEnumerable<CompanyVM>?>> AllCompaniesAsync()
        {

            var result = await unitOfWork.Repository<Company>().GetAsQuery()
                .Select(x => new CompanyVM
                {
                    Name = x.Name,
                    City = x.City,
                    Email = x.Email,
                    Id = x.Id
                }).ToListAsync();
            return Result<IEnumerable<CompanyVM>?>.Success(result);
        }



        public async Task<Result> CreateCompanyAsync(CompanyInfoVM obj)
        {
            if (obj is null)
                return Result.Failure("Invalid input data", errorType: ErrorType.VALIDATION);

            var exists = await unitOfWork.Repository<Company>()
                .GetAsQuery()
                .AnyAsync(x => x.Email == obj.Email);

            if (exists)
                return Result.Failure("Email already exists", errorType: ErrorType.VALIDATION);

            var company = new Company
            {
                Name = obj.Name,
                Email = obj.Email,
                City = obj.City,
                PostalCode = obj.PostalCode,
                Address = obj.Address,
                Phone = obj.Phone,
                DiscountPercentage = obj.DiscountPercentage
            };

            await unitOfWork.Repository<Company>().AddAsync(company);
            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result<CompanyInfoVM?>> CompanyDetailsAsync(int id)
        {
            if (id <= 0)
                return Result<CompanyInfoVM?>.Failure(
                    "Invalid company Id",
                    "INVALID_COMPANY_ID",
                    ErrorType.NOT_FOUND);

            var company = await unitOfWork.Repository<Company>().FindAsync(id);

            if (company == null)
                return Result<CompanyInfoVM?>.Failure(
                    "Company not found.",
                    errorType: ErrorType.NOT_FOUND);

            var vm = new CompanyInfoVM
            {
                Id = company.Id,
                Name = company.Name,
                Email = company.Email,
                City = company.City,
                PostalCode = company.PostalCode,
                Address = company.Address,
                Phone = company.Phone,
                DiscountPercentage = company.DiscountPercentage
            };

            return Result<CompanyInfoVM?>.Success(vm);
        }

        public async Task<Result> DeleteCompanyAsync(int id)
        {
            if (id <= 0)
                return Result.Failure("Company Not Found", errorType: ErrorType.NOT_FOUND);
            var Company = await unitOfWork.Repository<Company>().FindAsync(id);
            if (Company == null)
                return Result.Failure("Company Not Found", errorType: ErrorType.NOT_FOUND);
            var isUserHas = await unitOfWork.Repository<ApplicationUser>()
                   .GetAsQuery()
                 .AnyAsync(x => x.CompanyId == id);

            if (isUserHas)
                return Result.Failure(
                    "Cannot delete company because it has users assigned to it.",
                    errorType: ErrorType.VALIDATION);

            unitOfWork.Repository<Company>().Delete(Company);

            return await unitOfWork.SaveChangesAsync() > 0 ? Result.Success()
                : Result.Failure("Failed to delete Company.", errorType: ErrorType.INTERNAL_ERROR);
        }



        public async Task<IEnumerable<SelectListItem>> GetAllCategoriesItems()
        {
            var categories = await unitOfWork.Repository<Company>().GetAllAsync();
            return categories.Select(c => new SelectListItem
            {
                Text = c.Name.Trim(),
                Value = c.Id.ToString()
            });
        }

        public async Task<Result> UpdateCompanyAsync(int id, CompanyInfoVM Obj)
        {
            if (id <= 0)
                return Result.Failure("Company not found.", errorType: ErrorType.NOT_FOUND);
            if (Obj is null)
                return Result.Failure("Invalid Company data.", errorType: ErrorType.VALIDATION);

            var company = await unitOfWork.Repository<Company>().FindAsync(id);
            if (company == null)
                return Result.Failure("Company not found.", errorType: ErrorType.NOT_FOUND);


            var companyWithSameName = await unitOfWork.Repository<Company>()
                          .AnyAsync(c => c.Name.ToLower() == Obj.Name.Trim().ToLower()
                             && c.Id != id);
            if (companyWithSameName)
                return Result.Failure("A Company with the same name already exists.", "Company_NAME_EXISTS", ErrorType.VALIDATION);

            var existsWithSameEmail = await unitOfWork.Repository<Company>()
      .AnyAsync(c => c.Email.ToLower() == Obj.Email.Trim().ToLower()
                  && c.Id != id);
            if (existsWithSameEmail)
                return Result.Failure(
                    "A company with the same email already exists.",
                    "COMPANY_EMAIL_EXISTS",
                    ErrorType.VALIDATION);

            company.Name = Obj.Name.Trim();
            company.Email = Obj.Email.Trim();
            company.City = Obj.City?.Trim();
            company.PostalCode = Obj.PostalCode?.Trim();
            company.Address = Obj.Address?.Trim();
            company.DiscountPercentage = Obj.DiscountPercentage;
            company.Phone = Obj.Phone;

            return await unitOfWork.SaveChangesAsync() > 0 ?
                Result.Success() : Result.Failure("Failed to update Company.", errorType: ErrorType.INTERNAL_ERROR);
        }

        public async Task<Result<IEnumerable<CompanyVM>?>> SearchAsync(string? search)
        {
            var query = unitOfWork.Repository<Company>()
                .GetAsQuery();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Name.Contains(search));
            }

            var data = await query.Select(x => new CompanyVM
            {
                Id = x.Id,
                Name = x.Name,
                City = x.City,
                Email = x.Email
            }).ToListAsync();

            return Result<IEnumerable<CompanyVM>?>.Success(data);
        }
    }
}
