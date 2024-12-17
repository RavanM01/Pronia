using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaTask.DAL;
using ProniaTask.Models;

namespace ProniaTask.Controllers
{
    public class ShopController : Controller
    {
        AppDBContext _context;

        public ShopController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? search)
        {
            List<Product> products;
            if (search != null)
            {
                products = await _context.Products.Include(x => x.ProductImages).Where(p => p.Name.ToLower().Contains(search.ToLower())).ToListAsync();
            }
            else
            {
                products=await _context.Products.Include(x=>x.ProductImages).ToListAsync();
            }
            return View(products);
        }

    }
}
