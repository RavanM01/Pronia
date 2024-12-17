using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaTask.DAL;
using ProniaTask.Models;

namespace ProniaTask.ViewComponents
{
    public class ProductViewComponent:ViewComponent
    {
        AppDBContext _context;

        public ProductViewComponent(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(string key)
        {
            List<Product> products = new List<Product>();
            switch (key.ToLower())
            {
                case "latest":
                    products=await _context.Products.OrderByDescending(x => x.Id).Take(4).ToListAsync();
                    break;
                case "featured":
                    products = await _context.Products.OrderByDescending(x => x.Price).Take(4).ToListAsync();
                    break;
                case "bestseller":
                    products = await _context.Products.OrderBy(x => x.Count).Take(4).ToListAsync();
                    break;
            }
            return View(products);
        }
    }
}
