using ComputerNetworksProject.Data;
using ComputerNetworksProject.Models;
using ComputerNetworksProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerNetworksProject.Controllers
{
    [ServiceFilter(typeof(CartFilter))]
    public class CategoriesController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _db = context;

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            //var categories = await _db.Categories.Where(c=>c.Name!="Default").ToListAsync();
            var categories = await _db.Categories.ToListAsync();
            var model = new CategoriesModel
            {
                Categories = categories,
                Input = new Category(),
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(CategoriesModel? data)
        {
            if(data is null)
            {
                return NotFound();
            }
            //if (_db.Categories.Where(c => c.Name == data.Input.Name).Any())
            //{
            //    ModelState.AddModelError("Input.Name", "Category name must be unique!");
            //}
            if (_db.Categories.Where(c => c.Name.ToLower() == data.Input.Name.ToLower()).Any())
            {
                ModelState.AddModelError("Input.Name", $"Category {data.Input.Name} already exists!");
            }
            if (ModelState.IsValid)
            {

                try
                {
                    _db.Add(data.Input);
                    await _db.SaveChangesAsync();
                    var categories = await _db.Categories.ToListAsync();
                    var model = new CategoriesModel
                    {
                        Categories = categories,
                        Input = new Category(),
                    };
                    TempData["info"] = $"{data.Input.Name} added successfully!";
                    return RedirectToAction("Index");
                }
                catch(DbUpdateException ex) 
                {
                    TempData["Error"] = $"Category {data.Input.Name} already exists!";
                }
            }
            var categories2 = await _db.Categories.ToListAsync();
            data.Categories = categories2;
            return View(data);

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string? name)
        {
            if(name is not null)
            {
                if(name == "Default")
                {
                    return Unauthorized();
                }
                var category=await _db.Categories.FindAsync(name);
                if(category is null)
                {
                    TempData["Error"] = $"Category {name} is not valid";
                    return RedirectToAction(nameof(Index));
                }
                try
                {
                    _db.Remove(category);
                    var products=await _db.Products.Where(p=>p.CategoryName == name).ToListAsync();
                    products.ForEach((p) => { p.CategoryName = "Default"; });
                    await _db.SaveChangesAsync();
                    TempData["info"] = $"Category {name} Deleted successfully!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error have occured please try deleting again!";
                }

            }
            else
            {
                TempData["Error"] = "Error have occured please try deleting again!";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
