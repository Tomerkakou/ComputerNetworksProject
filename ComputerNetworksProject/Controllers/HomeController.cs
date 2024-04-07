using ComputerNetworksProject.Data;
using ComputerNetworksProject.Hubs;
using ComputerNetworksProject.Models;
using ComputerNetworksProject.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static ComputerNetworksProject.Models.HomeModel;

namespace ComputerNetworksProject.Controllers
{
   // [ServiceFilter(typeof(CartFilter))]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<ProductsHub> _hub;
        private HomeModel _homeModel;

        public HomeController(ILogger<HomeController> logger,ApplicationDbContext db, IHubContext<ProductsHub> hub)
        {
            _hub= hub;  
            _logger = logger;
            _db = db;
            _homeModel = new HomeModel([.. _db.Products.Where(p=>p.ProductStatus!=Product.Status.DELETED).Include(p => p.Rates).Include(p=>p.Category)]);
        }
        [HttpGet]
        public async Task<IActionResult> Index(int? page,string? sort,bool? table, string? msg)
        {
            if (msg is not null)
            {
                TempData["warning"] = msg;
            }
            _homeModel.FilterInput = HttpContext.Session.GetObject<Filter>("Filter");
            if(_homeModel.FilterInput is null)
            {
                _homeModel.FilterInput = new HomeModel.Filter { Rate = 0 };
            }
            else if(_homeModel.FilterInput.CategoryName is not null && !_db.Categories.Where(c=>c.Name== _homeModel.FilterInput.CategoryName).Any())
            {
                _homeModel.FilterInput.CategoryName = null;
            }
            if(table is not null)
            {
                HttpContext.Session.SetObject(nameof(table), table);
            }
            _homeModel.ShowTable = HttpContext.Session.GetObject<bool?>(nameof(table));
            page ??= 1;
            if (sort is not null)
            {
                HttpContext.Session.SetObject(nameof(sort), sort);
            }
            _homeModel.Sort = HttpContext.Session.GetObject<string?>(nameof(sort));
            try
            {
                _homeModel.ApplyFilters();
                _homeModel.ApplySort();
                _homeModel.InitPage((int)page);

            }catch(ArgumentException ex) {
                _logger.LogTrace(ex, "happend while genrating pages");
                return NotFound();
            }
            var categories = await _db.Categories.ToListAsync();
            var all = new Category
            {
                Name="ALL"      
            };
            categories.Insert(0, all);
            ViewData["CategoryName"] = new SelectList(categories, "Name", "Name");

            return View(_homeModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(HomeModel homeModel)
        {
            if(ModelState.IsValid)
            {
                HttpContext.Session.SetObject("Filter", homeModel.FilterInput);
                return RedirectToAction(nameof(Index));
            }
            homeModel.Products=await _db.Products.ToListAsync();
            homeModel.FilterdProducts = homeModel.Products;
            homeModel.InitPage(homeModel.ActivePage);
            var categories = await _db.Categories.ToListAsync();
            var all = new Category
            {
                Name = "ALL"
            };
            categories.Insert(0, all);
            ViewData["CategoryName"] = new SelectList(categories, "Name", "Name");
            return View(homeModel);
        }

        public IActionResult ResetFilter()
        {
            HttpContext.Session.ClearObject("Filter");
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> GetProductCardParital(int? productId,string? type = "card")
        {
            if(productId is null)
            {
                return BadRequest("null productId");
            }
            var product=await _db.Products.FindAsync(productId);
            if(product is null)
            {
                return BadRequest("not valid product id");
            }
            if (type == "tr")
            {
                return PartialView("_ProductTableRowPartial",product);
            }
            return PartialView("_ProductCardPartial", product);
        }
    }
}