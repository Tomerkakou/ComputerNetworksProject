using ComputerNetworksProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerNetworksProject.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        public CartController(ApplicationDbContext db, SignInManager<User> signInManager, UserManager<User> userManager)
        {
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
                cart.AddProduct(product);
                await _db.SaveChangesAsync();
            }catch (ArgumentException ex)
            {
                return Unauthorized(ex.Message);
            }catch(DbUpdateException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
