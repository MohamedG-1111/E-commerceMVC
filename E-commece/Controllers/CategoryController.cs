using BLL.ViewModels;
using DataAcessLayer.Data;
using DataAcessLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_commece.Controllers
{
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        public CategoryController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var categories = _context.Categories.ToList();
            return View(categories);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(CategoryVM obj)
        {
            if (!ModelState.IsValid)
                return View(obj);
            Category category = new Category
            {
                Name = obj.Name,
                DisplayOrder = obj.DisplayOrder
            };

            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.Find(id);
            CategoryVM categoryVM = new CategoryVM
            {
                Name = category.Name,
                DisplayOrder = category.DisplayOrder
            };

            return View(categoryVM);
        }
        [HttpPost]
        public IActionResult Edit(int id, CategoryVM obj)
        {
            if (id <= 0)
                return NotFound();
            if (!ModelState.IsValid)
                return View(obj);
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();
            category.Name = obj.Name;
            category.DisplayOrder = obj.DisplayOrder;

            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();
            CategoryVM categoryVM = new CategoryVM
            {
                Name = category.Name,
                DisplayOrder = category.DisplayOrder
            };
            ViewBag.Id = id;
            return View(categoryVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult ConfirmDelete(int? id)
        {
            if (id <= 0)
                return NotFound();

            var category = _context.Categories.Find(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
