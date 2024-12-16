using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaTask.Areas.Manage.ViewModels;
using ProniaTask.Areas.Manage.ViewModels.Product;
using ProniaTask.DAL;
using ProniaTask.Helpers.Extensions;
using ProniaTask.Models;
using System.Collections.Generic;

namespace ProniaTask.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        AppDBContext dbContext;
        private readonly IWebHostEnvironment env;

        public ProductController(AppDBContext dbContext,IWebHostEnvironment env)
        {
            this.dbContext = dbContext;
            this.env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await dbContext.Products.Include(x=>x.Category)
                .Include(c=>c.TagProducts)
                .ThenInclude(z=>z.Tag)
                .Include(x=>x.ProductImages)
                .ToListAsync();
            
            
            return View(products);
        }
        
        public IActionResult Create()
        {
            ViewBag.Categories= dbContext.Categories.ToList();
            ViewBag.Tags= dbContext.Tags.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVm vm)
        {
                ViewBag.Categories = dbContext.Categories.ToList();
            ViewBag.Tags = dbContext.Tags.ToList();
            if (!ModelState.IsValid)
            {

                return View();
            }
            if (vm.CategoryId != null)
            {
                if(!dbContext.Categories.Any(c=>c.Id == vm.CategoryId))
                {
                    ModelState.AddModelError("CategoryId", $"{vm.CategoryId}-idli category yoxdur");
                    return View();
                }
            }




            Product product= new Product()
            {
                Name = vm.Name,
                Price = vm.Price,
                Description = vm.Description,
                SKU = vm.SKU,
                CategoryId = vm.CategoryId,
                ProductImages=new List<ProductImage>()
            };


            if (vm.TagIds != null)
            {
                foreach (var tagId in vm.TagIds)
                {
                    if (!(await dbContext.Tags.AnyAsync(t => t.Id == tagId)))
                    {
                        ModelState.AddModelError("TagIds", "Id li tag yoxdur");
                        return View();
                    }
                    TagProduct tagProduct = new TagProduct()
                    {
                        TagId = tagId,
                        Product = product
                    };  
                    dbContext.TagProducts.Add(tagProduct);
                     
                }
            }
            List<string> error = new List<string>();
            if (!vm.MainPhoto.ContentType.Contains("image/"))
            {
                ModelState.AddModelError("MainPhoto", "Sekil daxil edin");
                return View();
            }
            if (vm.MainPhoto.Length > 3000000)
            {
                ModelState.AddModelError("MainPhoto", "Max 2mb ola biler");
                return View();
            }
            product.ProductImages.Add(new()
            {
                Primary=true,
                ImgUrl=vm.MainPhoto.Upload(env.WebRootPath,"Upload/product")
            });

            foreach (var image in vm.Photos)
            {
                if (!image.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("Photos", "Sekil daxil edin");
                    return View();
                }
                if (image.Length > 3000000)
                {
                    ModelState.AddModelError("Photos", "Max 2mb ola biler");
                    return View();
                }
                product.ProductImages.Add(new()
                {
                    Primary = false,
                    ImgUrl =image.Upload(env.WebRootPath, "Upload/product")
                });
            }

            TempData["error"]=error;


            await dbContext.Products.AddAsync(product);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int? id)
        {
            ViewBag.Tags = dbContext.Tags.ToList();

            if (id == null)
            {
                return NotFound();
            }
            var product = await dbContext.Products.Include(x => x.Category)
                .Include(p=>p.ProductImages)
                .Include(c => c.TagProducts)
                .ThenInclude(z => z.Tag)
                .FirstOrDefaultAsync(g=>g.Id==id);
            ViewBag.Categories = dbContext.Categories.ToList();
            UpdateProductVm updateProVm = new UpdateProductVm()
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                CategoryId = product.CategoryId,
                TagIds = new List<int>(),
                productImages=new List<ProductImageVm>()

            };
            foreach (var item in product.TagProducts)
            {
                updateProVm.TagIds.Add(item.Id);
            }
            foreach (var item in product.ProductImages)
            {
                updateProVm.productImages.Add(new() {   
                Primary=item.Primary,
                ImgUrl=item.ImgUrl,
                });
            }
            return View(updateProVm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductVm vm)
        {
            if (vm.Id == null)
            {
                return NotFound();
            }
           if (!ModelState.IsValid)
            {
                return View(vm);
            }
            Product oldProduct= dbContext.Products.Include(p=>p.TagProducts).Include(p=>p.ProductImages).FirstOrDefault(x=>x.Id==vm.Id);
            if (oldProduct == null)
            {
                return NotFound();
            }
            if (vm.CategoryId != null)
            {
                if (!dbContext.Categories.Any(c => c.Id == vm.CategoryId))
                {
                    ModelState.AddModelError("CategoryId", $"{vm.CategoryId}-idli category yoxdur");
                    return View();
                }
            }


            if (vm.TagIds!=null)
            {
            dbContext.TagProducts.RemoveRange(oldProduct.TagProducts);

                foreach (var item in vm.TagIds)
                {
                    await dbContext.TagProducts.AddAsync(new TagProduct()
                    {
                        ProductId = oldProduct.Id,
                        TagId=item
                    });
                    
                }

            }


            if (vm.MainPhoto != null)
            {
                if (!vm.MainPhoto.ContentType.Contains("image/"))
                {
                    ModelState.AddModelError("MainPhoto", "Sekil daxil edin");
                    return View(vm);
                }
                if (vm.MainPhoto.Length > 3000000)
                {
                    ModelState.AddModelError("MainPhoto", "Max 2mb ola biler");
                    return View(vm);
                }

                FileExtension.DeleteFile(env.WebRootPath, "Upload/product", oldProduct.ProductImages.FirstOrDefault(x => x.Primary).ImgUrl);
                dbContext.ProductImages.Remove(oldProduct.ProductImages.FirstOrDefault(x => x.Primary));

                oldProduct.ProductImages.Add(new()
                {
                    Primary = true,
                    ImgUrl = vm.MainPhoto.Upload(env.WebRootPath, "Upload/product")
                });

               

            }

            if (vm.ImagesUrls != null)
            {
                var removeImgs=new List<ProductImage>();
                foreach(var imgUrl in oldProduct.ProductImages.Where(x=>!x.Primary))
                {
                    if (!vm.ImagesUrls.Any(x => x == imgUrl.ImgUrl))
                    {
                        FileExtension.DeleteFile(env.WebRootPath,"Upload/product",imgUrl.ImgUrl);
                        dbContext.ProductImages.Remove(imgUrl);
                    }
                    
                }
            }
            else
            {
                foreach (var item in oldProduct.ProductImages.Where(x=>!x.Primary))
                {
                    FileExtension.DeleteFile(env.WebRootPath, "Upload/product", item.ImgUrl);
                    dbContext.ProductImages.Remove(item);
                }
            }
            if (vm.Images != null) {
                foreach (var image in vm.Images)
                {
                    if (!image.ContentType.Contains("image/"))
                    {
                        ModelState.AddModelError("Photos", "Sekil daxil edin");
                        return View();
                    }
                    if (image.Length > 3000000)
                    {
                        ModelState.AddModelError("Photos", "Max 2mb ola biler");
                        return View();
                    }
                    oldProduct.ProductImages.Add(new()
                    {
                        Primary = false,
                        ImgUrl = image.Upload(env.WebRootPath, "Upload/product")
                    });
                }
            }
            oldProduct.Name = vm.Name;
            oldProduct.Price = vm.Price;
            oldProduct.Description = vm.Description;
            oldProduct.CategoryId = vm.CategoryId;
            await dbContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = dbContext.Products.FirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();
            dbContext.Products.Remove(product);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
