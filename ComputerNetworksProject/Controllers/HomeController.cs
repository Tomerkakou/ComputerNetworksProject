using ComputerNetworksProject.Data;
using ComputerNetworksProject.Models;
using ComputerNetworksProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static ComputerNetworksProject.Models.HomeModel;

namespace ComputerNetworksProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private HomeModel _homeModel;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
            _homeModel = new HomeModel(_db.Products.Include(p => p.Rates).ToList());
        }

        public async Task<IActionResult> Index(int? page)
        {
            _homeModel.FilterInput = HttpContext.Session.GetObject<Filter>("Filter");
            if (page is not null)
            {
                try
                {
                    _homeModel.InitPage((int)page);

                }catch(ArgumentException ex) {
                    return NotFound();
                }
            }
            var categories = await _db.Categories.ToListAsync();
            var all = new Category
            {
                Id = 0,
                Name="ALL"      
            };
            categories.Insert(0, all);
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");

            return View(_homeModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(HomeModel homeModel)
        {
            if(ModelState.IsValid)
            {
                HttpContext.Session.SetObject("Filter", homeModel.FilterInput);
                return RedirectToAction("Index", "Home", new { httpMethod = "GET" });
            }
            homeModel.Products=await _db.Products.ToListAsync();
            homeModel.FilterdProducts = homeModel.Products;
            homeModel.InitPage(homeModel.ActivePage);
            return View(homeModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}