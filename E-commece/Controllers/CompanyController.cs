using E_commerce.BLL.Services.Interfaces;
using E_commerce.BLL.ViewModels;
using Ecommerce.Utility;
using Ecommerce.Utility.ResultPattern;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    [Authorize(Roles = Roles.Admin)]
    public class CompanyController : AppController
    {
        private readonly ICompanyService companyService;

        public CompanyController(ICompanyService companyService)
        {
            this.companyService = companyService;
        }

        public async Task<IActionResult> Index()
        {
            var result = await companyService.AllCompaniesAsync();

            if (result.IsFailure)
                return HandleResult(result);

            return View(result.Value);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyInfoVM model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await companyService.CreateCompanyAsync(model);
            if (result.IsFailure)
                return HandleResult(result, nameof(Create), model);
            TempData["Success"] = "Company Created Successully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await companyService.CompanyDetailsAsync(id);
            if (result.IsFailure)
                return HandleResult(result);
            return View(result.Value);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, CompanyInfoVM obj)
        {
            if (id != obj.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(obj);
            var result = await companyService.UpdateCompanyAsync(id, obj);
            if (result.IsFailure)
                return HandleResult(result, nameof(Edit), obj);
            TempData["success"] = "Company Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string? search)
        {
            var result = await companyService.SearchAsync(search);
            return PartialView("_CompanyTable", result.Value);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await companyService.CompanyDetailsAsync(id);
            if (result.IsFailure) return HandleResult(result);
            return View(result.Value);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var result = await companyService.DeleteCompanyAsync(id);
            if (result.IsFailure)
            {
                if (result.ErrorType == ErrorType.VALIDATION)
                {
                    TempData["error"] = result.ErrorMessage;
                    return RedirectToAction(nameof(Index));
                }
                return HandleResult(result);
            }
            TempData["success"] = "Company Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }

}



