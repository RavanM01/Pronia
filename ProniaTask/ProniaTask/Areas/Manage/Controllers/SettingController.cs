using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaTask.DAL;
using ProniaTask.Models;

namespace ProniaTask.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SettingController : Controller
    {
        AppDBContext _context;

        public SettingController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _context.Settings.ToDictionaryAsync(x=>x.Key,x=>x.Value);
            return View(settings);
        }
        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Create(Setting setting)
        {
            _context.Settings.Add(setting);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(string? key)
        {
            if (key == null) { return NotFound(); }
            var setting = _context.Settings.FirstOrDefault(x => x.Key == key);
            if (setting == null)
            {
                return NotFound();
            }
            _context.Settings.Remove(setting);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Update(string? key)
        {
            if (key == null) return BadRequest();
            var category = _context.Settings.FirstOrDefault(x => x.Key == key);
            if (category == null) return NotFound();
            return View(category);
        }
        [HttpPost]
        public IActionResult Update(Setting setting)
        {
            if (setting.Key == null) return BadRequest();
            var oldSetting = _context.Settings.FirstOrDefault(x => x.Key == setting.Key);
            if (oldSetting == null) return NotFound();
            oldSetting.Value = setting.Value;
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
