using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShoping.Models.DatabaseModel;
using OnlineShoping.Models.ViewModel;
using OnlineShoping.Services;

namespace PrinceOnlineShopping.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IHostingEnvironment hostingEnvironment;

        [Obsolete]
        public ProductsController(AppDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.hostingEnvironment = hostingEnvironment;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Products.Include(p => p.Category);
            var appDbContext1 = appDbContext.OrderByDescending(p => p.AddedDateTime);
            return View(await appDbContext1.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category).Include(p => p.ProductImage)
                .Include(p => p.ProductColor).ThenInclude(p => p.Color)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName");
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProductViewModel AddProduct)
        {

            if (ModelState.IsValid)
            {
                AddProduct.Product.AddedDateTime = DateTime.Now;

                _context.Add(AddProduct.Product);
                await _context.SaveChangesAsync();
                var Id = AddProduct.Product.ProductId;

                if (Id > 0)
                {
                    for (int i = 0; i < AddProduct.SelectedColorid.Length; i++)
                    {
                        ProductColor color = new ProductColor();
                        color.ColorId = AddProduct.SelectedColorid[i];
                        color.ProductId = Id;
                        _context.Add(color);

                    }
                    if (AddProduct.Photos != null && AddProduct.Photos.Count > 0)
                    {
                        // Loop thru each selected file
                        foreach (IFormFile photo in AddProduct.Photos)
                        {

                            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "images");

                            string uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await photo.CopyToAsync(stream);
                            }
                            ProductImage productImage = new ProductImage()
                            {
                                FileName = uniqueFileName,
                                ProductId = Id
                            };
                            _context.Add(productImage);
                        }

                    }




                    await _context.SaveChangesAsync();
                }


                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", AddProduct.Product.CategoryId);
            ViewData["ColorId"] = new SelectList(_context.Colors, "ColorId", "ColorName");
            return View(AddProduct);
        }

        // GET: Admin/Products/Edit/5

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["ProductSize"] = new SelectList(_context.Category, "CategoryId", "CategoryName", product.ProductSize);
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ShortDescription,LongDescription,Price,Gender,ProductSize,Stock,Discount,CategoryId")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.AddedDateTime = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }



    }
}
