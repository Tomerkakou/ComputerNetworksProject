using ComputerNetworksProject.Data;
using ComputerNetworksProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerNetworksProject.Controllers
{
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
                    return View(model);
                }
                catch
                {
                    TempData["Error"] = "Error have occured please try deleting again!";
                }
            }
            var categories2 = await _db.Categories.ToListAsync();
            data.Categories = categories2;
            return View(data);

        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if(id is not null)
            {
                var category=await _db.Categories.FindAsync(id);
                if(category is null)
                {
                    TempData["Error"] = $"Category id {id} is not valid";
                    return RedirectToAction(nameof(Index));
                }
                try
                {
                    _db.Remove(category);
                    var products=await _db.Products.Where(p=>p.CategoryId== id).ToListAsync();
                    products.ForEach((p) => { p.CategoryId = 1; });
                    await _db.SaveChangesAsync();
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
            TempData["info"] = "Deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
