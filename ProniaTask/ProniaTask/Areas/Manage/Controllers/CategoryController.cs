using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaTask.DAL;
using ProniaTask.Models;

namespace ProniaTask.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        AppDBContext _dbContext;

        public CategoryController(AppDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _dbContext.Categories.Include(x=>x.Products).ToListAsync();
            return View(categories);
        }
        public IActionResult Create() { 
        
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category category)
        {
            _dbContext.Categories.Add(category);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id)
        {
            if (id == null) { return NotFound(); }
            var category = _dbContext.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(int id)
        {
            if (id == null) return BadRequest();
            var category = _dbContext.Categories.FirstOrDefault(x => x.Id == id);
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        public IActionResult Update(Category category)
        {
            if (category.Id == null) return BadRequest();
            var oldCategory = _dbContext.Categories.FirstOrDefault(x => x.Id == category.Id);
            if (oldCategory == null) return NotFound();
            oldCategory.Name = category.Name;
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
