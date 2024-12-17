using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProniaTask.DAL;
using ProniaTask.ViewModels.Basket;

namespace ProniaTask.ViewComponents
{
    public class BasketViewComponent:ViewComponent
    {
        AppDBContext _context;

        public BasketViewComponent(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
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
                cookies.ForEach(c =>
                {
                    var product = _context.Products.Include(x => x.ProductImages).FirstOrDefault(x => x.Id == c.Id);
                    if (product == null)
                    {
                        deleteItem.Add(c);


                    }
                    else
                    {
                        cart.Add(new CartVm()
                        {
                            Id = c.Id,
                            Name = product.Name,
                            Price = product.Price,
                            ImgUrl = product.ProductImages.FirstOrDefault(p => p.Primary).ImgUrl,
                            Count = c.Count
                        });
                    }
                });
                if (deleteItem.Count > 0)
                {
                    deleteItem.ForEach(d =>
                    {
                        cookies.Remove(d);
                    });
                    HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookies));
                }
            }
            return View(cart);
        }
    }
}
