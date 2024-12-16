using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaTask.DAL;
using ProniaTask.Helpers.Extensions;
using ProniaTask.Models;

namespace ProniaTask.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController : Controller
    {
        AppDBContext _context;
        private readonly IWebHostEnvironment env;

        public SliderController(AppDBContext context,IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        public async Task<IActionResult> Index()
        {
            List<Slider> sliders= await _context.sliders.ToListAsync(); 
            return View(sliders);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Slider slider)
        {
            if (!slider.File.ContentType.Contains("image"))
            {
                ModelState.AddModelError("File", "fayl image deyil");
                return View();
            }
            if (slider.File.Length > 2097152)
            {
                ModelState.AddModelError("File", "fayl image deyil");
                return View();
            }

            slider.ImgUrl = slider.File.Upload(env.WebRootPath,"Upload/slider");
            if (!ModelState.IsValid)
            {
                return View();
            }
            _context.sliders.Add(slider);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int? id) { 
            var slider = _context.sliders.FirstOrDefault(_context => _context.Id == id); 
            if (slider == null)
            {
                return NotFound();
            }
            FileExtension.DeleteFile(env.WebRootPath,"Upload/Slider",slider.ImgUrl);
            _context.sliders.Remove(slider);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
