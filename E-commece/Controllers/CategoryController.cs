using E_commece.Data;
using E_commece.Models;
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
        public IActionResult Create(Category obj)
        {
            _context.Categories.Add(obj);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
