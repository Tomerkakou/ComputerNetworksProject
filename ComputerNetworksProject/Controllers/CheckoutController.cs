using ComputerNetworksProject.Data;
using ComputerNetworksProject.Hubs;
using ComputerNetworksProject.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ComputerNetworksProject.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IHubContext<ProductsHub> _hub;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly AesEncrypter _aes;
        public CheckoutController(ApplicationDbContext db, IHubContext<ProductsHub> hub, SignInManager<User> signInManager, UserManager<User> userManager, AesEncrypter aes)
        {
            _db = db;
            _hub = hub;
            _signInManager = signInManager;
            _userManager = userManager;
            _aes = aes;
        }
        public async Task<IActionResult> Review(int cartId,int? shippingId)
        {
            var cart = await _db.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.Id == cartId && c.CartStatus == Cart.Status.ACTIVE);
            
            if(cart is null)
            {
                return BadRequest("cardId is not valid");
            }
            ViewBag.ShippingId = shippingId;
            return View(cart);
        }

        public async Task<IActionResult> Shipping(int cartId, int? shippingId)
        {
            var cart = await _db.Carts.FirstOrDefaultAsync(c => c.Id == cartId && c.CartStatus == Cart.Status.ACTIVE);
            if (cart is null)
            {
                return BadRequest("cardId is not valid");
            }
            Shipping? shipping=null;
            if (shippingId is not null)
            {
                shipping = await _db.Shippings.FindAsync(shippingId);
            }
            else if(_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is not null && user.ShippingId is not null)
                {
                    shipping = await _db.Shippings.FindAsync(user.ShippingId);
                }
            }
            ViewBag.CartId = cartId;
            return View(shipping  ?? new Shipping());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Shipping(int cartId,Shipping model)
        {
            var cart = await _db.Carts.FirstOrDefaultAsync(c => c.Id == cartId && c.CartStatus == Cart.Status.ACTIVE);
            if (cart is null)
            {
                return BadRequest("cardId is not valid");
            }
            if (ModelState.IsValid)
            {
                await _db.Shippings.AddAsync(model);
                if (model.Save && _signInManager.IsSignedIn(User)) {
                    var user = await _userManager.GetUserAsync(User);
                    if (user is not null)
                    {
                        user.SavedShipping = model;
                    }
                }
                await _db.SaveChangesAsync();
                return RedirectToAction("Payment",new { cartId, shippingId = model.Id });
            }
            ViewBag.CartId = cartId;
            return View(model);
        }
        public async Task<IActionResult> Payment(int cartId,int shippingId)
        {
            var cart = await _db.Carts.FirstOrDefaultAsync(c => c.Id == cartId && c.CartStatus == Cart.Status.ACTIVE);
            var shipping = await _db.Shippings.FindAsync(shippingId);
            if (cart is null || shipping is null)
            {
                return BadRequest("cardId is not valid or shippingId is not valid");
            }
            User? user = null;
            if (_signInManager.IsSignedIn(User))
            {
                user = await _userManager.GetUserAsync(User);
                if (cart.UserId is not null && cart.UserId != user.Id)
                {
                    TempData["error"] = $"Cart id {cart.Id} does not belong to that user!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (cart.UserId is not null)
            {
                TempData["error"] = $"Cart id {cart.Id} belong to another user!";
                return RedirectToAction("Index", "Home");
            }
            Payment? payment = new Payment();
            ViewBag.CartId = cartId;    
            ViewBag.ShippingId = shippingId;    
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Payment(int cartId,Payment payment, int shippingId)
        {
            var cart = await _db.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.Id == cartId && c.CartStatus == Cart.Status.ACTIVE);
            var shipping = await _db.Shippings.FindAsync(shippingId);
            if (cart is null || shipping is null)
            {
                return BadRequest("cardId is not valid or shippingId is not valid");
            }
            User? user = null;
            if (_signInManager.IsSignedIn(User))
            {
                user = await _userManager.GetUserAsync(User);
                if (cart.UserId is not null && cart.UserId != user.Id)
                {
                    TempData["error"] = $"Cart id {cart.Id} does not belong to that user!";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (cart.UserId is not null)
            {
                TempData["error"] = $"Cart id {cart.Id} belong to another user!";
                return RedirectToAction("Index", "Home");
            }
            if (ModelState.IsValid)
            {
                cart.CompleteCart();
                payment.CreditCardNumberEncrypt= _aes.Encrypt(payment.CreditCardNumber);
                await _db.Payments.AddAsync(payment);
                var order = new Order
                {
                    Cart = cart,
                    Shipping = shipping,
                    Payment = payment,
                };

                if (user is not null)
                {
                    order.UserId= user.Id;
                    if (payment.Save)
                    {
                        user.SavedPayment = payment;
                    }
                }
                await _db.Orders.AddAsync(order);
                await _db.SaveChangesAsync();

                await _hub.Clients.All.SendAsync("clearCart", cart.Id);

                HttpContext.Response.Cookies.Delete("cart_id");
                return RedirectToAction("Completed",new {orderId=order.Id});
            }
            ViewBag.CartId = cartId;
            ViewBag.ShippingId = shippingId;
            return View(payment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> SavedPayment(int cartId, int shippingId,string userId)
        {
            var cart = await _db.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.Id == cartId && c.CartStatus == Cart.Status.ACTIVE);
            var shipping = await _db.Shippings.FindAsync(shippingId);
            if (cart is null || shipping is null)
            {
                return BadRequest("cardId is not valid or shippingId is not valid");
            }
            if (!_signInManager.IsSignedIn(User))
            {
                return Unauthorized("user is not loggedin");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user.Id != userId)
            {
                return Unauthorized("userId is not matching");
            }
            if (cart.UserId is null || cart.UserId != user.Id)
            {
                TempData["error"] = $"Cart id {cart.Id} does not belong to that user!";
                return RedirectToAction("Index", "Home");
            }
            if (user is null || user.PaymentId is null)
            {
                return BadRequest("No saved payment for user!");
            }
            var temp = await _db.Payments.FindAsync(user.PaymentId);
            var payment = new Payment(temp);
            cart.CompleteCart();
            await _db.Payments.AddAsync(payment);
            var order = new Order
            {
                Cart = cart,
                Shipping = shipping,
                Payment = payment,
            };
            order.UserId = user.Id;
            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync();

            await _hub.Clients.All.SendAsync("clearCart", cart.Id);

            HttpContext.Response.Cookies.Delete("cart_id");
            return RedirectToAction("Completed", new { orderId = order.Id });
        }
        public async Task<IActionResult> Completed(int orderId)
        {
            var order= await _db.Orders.Include(o=>o.Cart).ThenInclude(c=>c.CartItems).ThenInclude(ci=>ci.Product).FirstAsync(o=>o.Id==orderId);
            return View(order);
        }
    }
}
