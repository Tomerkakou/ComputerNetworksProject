using ComputerNetworksProject.Data;
using ComputerNetworksProject.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ComputerNetworksProject.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IHubContext<ProductsHub> _hub;
        public CartController(ApplicationDbContext db, SignInManager<User> signInManager, UserManager<User> userManager, IHubContext<ProductsHub> hub)
        {
            _hub = hub;
            _db = db;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> AddItem(int? productId)
        {
            if(productId is null)
            {
                return BadRequest("product id is null");
            }
            var product =await _db.Products.FirstOrDefaultAsync(p=>p.Id == productId);

            if(product is null) {
                return BadRequest("not valid product id");
            }
            Cart? cart = (Cart?)ViewData["Cart"];
            if(cart is null)
            {
                cart=new Cart();
                await _db.Carts.AddAsync(cart);
            }
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is not null)
                {
                    cart.UserId=user.Id;
                }
            }
            try
            {
                var cartItemAmount=cart.AddProduct(product);
                await _db.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("productNewAvailableStock", productId, product.AvailableStock, cartItemAmount);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return Unauthorized(ex.Message);
            }catch(DbUpdateException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public async Task<IActionResult> DecreaseItem(int? productId)
        {
            Cart? cart = (Cart?)ViewData["Cart"];
            if (productId is null)
            {
                return BadRequest("No productId");
            }
            var product=await _db.Products.FindAsync(productId);
            if(cart is null)
            {
                return BadRequest("No cart");
            }
            try
            {
                var cartItemAmount = cart.DecreaseItemAmount((int)productId);
                await _db.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("productNewAvailableStock", productId, product.AvailableStock,cartItemAmount);
                return Ok();
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest("NO such product in cart");
            }
        }
    }
}
