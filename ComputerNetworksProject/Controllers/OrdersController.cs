using ComputerNetworksProject.Data;
using ComputerNetworksProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ComputerNetworksProject.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly AesEncrypter _aes;
        private readonly UserManager<User> _userManager;

        public OrdersController(ApplicationDbContext db, AesEncrypter aes, UserManager<User> userManager)
        {
            _db = db;
            _aes = aes;
            _userManager = userManager;
        }

        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            if(userId is null)
            {
                return NotFound();  
            }
            var user = await _db.Users.Include(u => u.Orders)
                .ThenInclude(o => o.Cart).ThenInclude(c=>c.CartItems).ThenInclude(ci=>ci.Product)
                .Include(u => u.Orders)
                .ThenInclude(o => o.Shipping)
                .Include(u => u.Orders)
                .ThenInclude(o => o.Payment)
                .FirstAsync(u=>u.Id==userId);

            float sum = 0;
            foreach (var order in user.Orders)
            {
                order.Payment.CreditCardNumber = _aes.Decrypt(order.Payment.CreditCardNumberEncrypt);
                sum += order.Cart.GetTotalPrice();
            }
            ViewBag.TotalOrderPrice = sum;
            ViewBag.TotalOrdersCount = user.Orders.Count;
            return View(user);
        }
    }
}
