using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaTask.DAL;
using ProniaTask.Models;
using ProniaTask.ViewModels;

namespace ProniaTask.Controllers
{
    public class HomeController : Controller
    {
        AppDBContext dbContext;
        public HomeController(AppDBContext appDBContext)
        {
            dbContext = appDBContext;
        }
        public IActionResult Index()
        {
            List<Slider> sliders = dbContext.sliders.ToList();
            List<Product> products = dbContext.Products.Include(p=>p.ProductImages).ToList();

            HomeVM vm = new HomeVM()
            {
                Sliders = sliders,
                Products = products,
            };
            return View(vm);
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if(id== null)
            {
                return NotFound();
            }
            var product =await dbContext.Products.Include(x=>x.Category)
                .Include(x => x.ProductImages)
                .Include(x => x.TagProducts)
                .ThenInclude(x => x.Tag)
                .FirstOrDefaultAsync(p=>p.Id==id);

            // List<Product> products = dbContext.Products.Include(p => p.ProductImages).ToList();
            ViewBag.ReProduct = await dbContext.Products.Include(x => x.ProductImages).Where(x => x.CategoryId == product.CategoryId).ToListAsync();


            return View(product);
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
    }
}
