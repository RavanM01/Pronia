using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaTask.DAL;
using ProniaTask.ViewModels.Basket;
using System.Collections.Generic;

namespace ProniaTask.Controllers
{
    public class CartController : Controller
    {
        AppDBContext _context;

        public CartController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var json = Request.Cookies["basket"];
            List<CookieItemVm> cookies = new List<CookieItemVm>();
            if (json != null)
            {
                cookies = JsonConvert.DeserializeObject<List<CookieItemVm>>(json);

            }
            List<CartVm> cart = new List<CartVm>();
            List<CookieItemVm> deleteItem = new List<CookieItemVm>();
            if (cookies.Count > 0)
            {
                cookies.ForEach( c =>
                {
                    var product =  _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == c.Id);
                    if (product == null)
                    {
                        deleteItem.Add(c);
                

                    }
                    else
                    {
                        cart.Add(new CartVm()
                        {
                            Id=c.Id,
                            Name=product.Name,
                            Price =product.Price,
                            ImgUrl=product.ProductImages.FirstOrDefault(p=>p.Primary).ImgUrl,
                            Count=c.Count
                        });
                    }
                });
                if (deleteItem.Count > 0)
                {
                    deleteItem.ForEach(d =>
                    {
                        cookies.Remove(d);
                    });
                Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookies));
                }
            }
            return View(cart);
        }
        public async Task<IActionResult> AddBasket(int itemId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == itemId);
            if (product == null) return NotFound();

            List<CookieItemVm> cookiesList;

            var basket = Request.Cookies["basket"];
            if (basket != null)
            {
                cookiesList = JsonConvert.DeserializeObject<List<CookieItemVm>>(basket);
                var existsproduct = cookiesList.FirstOrDefault(x => x.Id == itemId);
                if (existsproduct != null)
                {
                    existsproduct.Count += 1;
                }
                else
                {
                    cookiesList.Add(new CookieItemVm() { 
                    Id=itemId,
                    Count=1
                   
                    });
                }
            }
            else
            {
                cookiesList = new List<CookieItemVm>();
            }
            CookieItemVm vm = new CookieItemVm()
            {
                Id=itemId,
                Count=1
            };


            var json = JsonConvert.SerializeObject(vm); 
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookiesList));

            return RedirectToAction("Index","Home");

        }
        public IActionResult GetBasket()
        {
            var json = JsonConvert.DeserializeObject<CookieItemVm>(Request.Cookies["basket"]);

            return View();
        }
    }
}
