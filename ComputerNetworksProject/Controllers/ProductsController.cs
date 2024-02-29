using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ComputerNetworksProject.Data;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.Extensions.Options;

namespace ComputerNetworksProject.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductsController(IWebHostEnvironment webHostEnvironment, ILogger<ProductsController> logger, IConfiguration config, ApplicationDbContext context)
        {
            _db = context;
            _logger = logger;
            _config = config;
            _webHostEnvironment = webHostEnvironment;
        }

        //Show product
        public async Task<IActionResult> Show(int? id)
        {
            if (id == null || _db.Products == null)
            {
                _logger.LogWarning("product id is null");
                return NotFound();
            }

            var product = await _db.Products.Include(p => p.Category).Include(p=>p.Rates).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                _logger.LogWarning("product id {0} is invalid", id);
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryName"] = new SelectList(_db.Categories, "Name", "Name");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(float price,float? priceDiscount,string name,string? description,string categoryName ,IFormFile? image,int stock)//[Bind("Id,Price,PriceDiscount,Name,Description,Stars,CategoryId,Img,ImgType")] Product product)
        {
            try
            {
                var product = new Product
                {
                    Name = name,
                    Description = description,
                    Price = price,
                    PriceDiscount = priceDiscount,
                    CategoryName = categoryName,
                    Stock = stock,
                    AvailableStock = stock,
                    Created=DateTime.Now,
                };
                if (priceDiscount >= price)
                {
                    ModelState.AddModelError("PriceDiscount", "Must be lower than price");
                }
                if (ModelState.IsValid)
                {
                    
                    using (var memoryStream = new MemoryStream())
                    {
                        var filePath=Path.Combine(_webHostEnvironment.WebRootPath, _config["defaultImageFile"]);

                        if (image != null && image.Length > 0)
                        {
                            image.CopyTo(memoryStream);
                        }
                        else if(System.IO.File.Exists(filePath))
                        {
                            using (var fileStream = System.IO.File.OpenRead((filePath)))
                            {
                                fileStream.CopyTo(memoryStream);
                            }
                        }
                        else
                        {
                            _logger.LogError("could not fine default image at path {0}", filePath);
                            return StatusCode(500, $"Internal server error");
                        }
                        byte[] imageD = memoryStream.ToArray();
                        string imageType = GetImageType(imageD);
                        product.Img = imageD;
                        product.ImgType= imageType;
                    }
                    _db.Add(product);
                    await _db.SaveChangesAsync();
                    TempData["success"] = $"Product {product.Name} created successfully!";
                    return RedirectToAction(nameof(Create));
                }
                ViewData["CategoryName"] = new SelectList(_db.Categories, "Name", "Name");
                return View(product);
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
            ViewData["CategoryName"] = new SelectList(_db.Categories, "Name", "Name", product.CategoryName);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id,float price, float? priceDiscount, string name, string description, string categoryName, IFormFile? image, int stock,DateTime created)
        {
            var product = await _db.Products.FindAsync(id);
            if(product == null)
            {
                return NotFound();
            }
            if (priceDiscount >= price)
            {
                ModelState.AddModelError("PriceDiscount", "Must be lower than price");
            }
            var orderedStock = product.Stock - product.AvailableStock;
            if (stock < orderedStock)
            {
                ModelState.AddModelError("Stock", $"{orderedStock} items already ordered must be greater!");
            }
            bool sendNotify = product.Stock < stock;
            if (ModelState.IsValid)
            {
                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        image.CopyTo(memoryStream);
                        byte[] imageD = memoryStream.ToArray();
                        string imageType = GetImageType(imageD);

                        product.Img = imageD;
                        product.ImgType= imageType; 
                    }
                }
                product.Name = name;
                product.Description = description;
                product.Price = price;
                product.PriceDiscount = priceDiscount;
                product.AvailableStock= stock- orderedStock;
                product.Stock = stock;
                product.CategoryName = categoryName;
                product.Created= created;
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
                if (sendNotify)
                {
                    //send email notifications
                }
                TempData["success"] = $"{name} updated successfully!";
                return RedirectToAction(nameof(Show), new { id = id });
            }
            ViewData["CategoryName"] = new SelectList(_db.Categories, "Name", "Name", product.CategoryName);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (_db.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _db.Products.FindAsync(id);
            if (product != null)
            {
                product.ProductStatus=Product.Status.DELETED;
            }
            
            await _db.SaveChangesAsync();
            return RedirectToAction("Index","Home");
        }

        //adding rating to product    
        public async Task<IActionResult> AddRating([FromQuery(Name = "productId")] int productId, [FromQuery(Name = "rate")]  int rate)
        {
            var product=await _db.Products.FindAsync(productId);
            if (product == null) {
                return NotFound();
            }
            await _db.Entry(product).Collection(p => p.Rates).LoadAsync();
            if (product.Rates== null)
            {
                product.Rates = new List<Rate>();
            }
            product.Rates.Add(new Rate
            {
                ProductId = productId,
                Stars = rate
            });
            product._rate = null;
            _db.SaveChanges();
            return Ok(product.Rate);
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
