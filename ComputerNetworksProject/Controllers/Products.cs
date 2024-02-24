using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComputerNetworksProject.Data;
using Microsoft.AspNetCore.Authorization;
using System.Web;

namespace ComputerNetworksProject.Controllers
{
    public class Products : Controller
    {
        private readonly ApplicationDbContext _db;

        public Products(ApplicationDbContext context)
        {
            _db = context;
        }

        // GET: Products/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(float price,float priceDiscount,string name,string description,int stars,int categortId ,IFormFile image)//[Bind("Id,Price,PriceDiscount,Name,Description,Stars,CategoryId,Img,ImgType")] Product product)
        {
            try
            {
                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        image.CopyTo(memoryStream);
                        byte[] imageD = memoryStream.ToArray();
                        string imageType = GetImageType(imageD);
                        var product = new Product
                        {
                            Name = name,
                            Description = description,
                            Price = price,
                            PriceDiscount = priceDiscount,
                            Stars = stars,
                            CategoryId = categortId+1,
                            ImgType = imageType,
                            Img = imageD
                        };
                        if (ModelState.IsValid)
                        {
                            _db.Add(product);
                            await _db.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        return View();
                    }
                }
                else
                {
                    var product = new Product
                    {
                        Name = name,
                        Description = description,
                        Price = price,
                        PriceDiscount = priceDiscount,
                        Stars = stars,
                        CategoryId = categortId+1,
                    };
                    if (ModelState.IsValid)
                    {
                        _db.Add(product);
                        await _db.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));
                    }
                    return View();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _db.Products == null)
            {
                return NotFound();
            }

            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Price,PriceDiscount,Name,Description,Stars,CategoryId,Img,ImgType")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _db.Update(product);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["CategoryId"] = new SelectList(_db.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _db.Products == null)
            {
                return NotFound();
            }

            var product = await _db.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_db.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                _db.Products.Remove(product);
            }
            
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_db.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private string GetImageType(byte[] imageData)
        {
            // Check the image file signature to determine the format
            if (imageData.Length >= 2 && imageData[0] == 0xFF && imageData[1] == 0xD8)
            {
                return "jpeg";
            }
            else if (imageData.Length >= 3 && imageData[0] == 0x89 && imageData[1] == 0x50 && imageData[2] == 0x4E)
            {
                return "png";
            }
            else if (imageData.Length >= 4 && imageData[0] == 0x47 && imageData[1] == 0x49 && imageData[2] == 0x46 && imageData[3] == 0x38)
            {
                return "gif";
            }
            else if (imageData.Length >= 2 && imageData[0] == 0x42 && imageData[1] == 0x4D)
            {
                return "bmp";
            }

            // Add more checks for other image formats as needed

            // Default to unknown type
            return "unknown";
        }
    }
}
