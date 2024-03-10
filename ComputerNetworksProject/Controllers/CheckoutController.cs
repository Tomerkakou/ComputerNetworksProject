using ComputerNetworksProject.Data;
using ComputerNetworksProject.Hubs;
using ComputerNetworksProject.Services;
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
        public async Task<IActionResult> Review(int? cartId)
        {
            Cart? cart;
            if(cartId is null)
            {
                cart = (Cart?)ViewData["Cart"];
            }
            else
            {
                cart = await _db.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.Id == cartId && c.CartStatus == Cart.Status.ACTIVE);
            }
            if(cart is null)
            {
                return NotFound();
            }
            TempData["checkout-cart2"] = cart.Id;
            TempData.Keep();
            return View(cart);
        }

        public async Task<IActionResult> Shipping()
        {
            if (!TempData.ContainsKey("checkout-cart2"))
            {
                return BadRequest();    
            }
            Shipping shipping = new Shipping();
            if (!TempData.ContainsKey("checkout-shipping2"))
            {
                if (_signInManager.IsSignedIn(User))
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user is not null && user.ShippingId is not null)
                    {
                        shipping = await _db.Shippings.FindAsync(user.ShippingId);
                    }
                }
            }
            else
            {
                shipping = JsonConvert.DeserializeObject<Shipping>((string)TempData["checkout-shipping2"]);
            }
            TempData.Keep();
            return View(shipping);
        }

        [HttpPost]
        public async Task<IActionResult> Shipping(Shipping model)
        {
            if (!TempData.ContainsKey("checkout-cart2"))
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                
                TempData["checkout-shipping2"] = JsonConvert.SerializeObject(model);
                return RedirectToAction("Payment");
            }
            TempData.Keep();
            return View(model);
        }
        public async Task<IActionResult> Payment(bool? useSavedPayment)
        {
            if (!TempData.ContainsKey("checkout-cart2") ||!TempData.ContainsKey("checkout-shipping2"))
            {
                return BadRequest();
            }
            Payment payment = new Payment();
            if (!TempData.ContainsKey("checkout-payment2")  && useSavedPayment is not null && (bool)useSavedPayment)
            {
                if (_signInManager.IsSignedIn(User))
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user is not null && user.ShippingId is not null)
                    {
                        var temp = await _db.Payments.FindAsync(user.SavedPayment);
                        if(temp is not null)
                        {
                            payment.Cvv = temp.Cvv;
                            payment.YearExp = temp.YearExp;
                            payment.MonthExp = temp.MonthExp;
                            payment.CreditCardNumberEncrypt = temp.CreditCardNumberEncrypt;
                        }           
                    }
                }
            }
            if(payment.CreditCardNumberEncrypt is not null)
            {
                payment.CreditCardNumber=_aes.Decrypt(payment.CreditCardNumberEncrypt);
            }
            TempData.Keep();
            return View(payment);
        }

        [HttpPost]
        public async Task<IActionResult> Payment(Payment payment)
        {
            if (!TempData.ContainsKey("checkout-cart2") || !TempData.ContainsKey("checkout-shipping2"))
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                payment.CreditCardNumberEncrypt= _aes.Encrypt(payment.CreditCardNumber);

                int cartId = (int)TempData["checkout-cart2"];
                var cart= await _db.Carts.Include(c => c.CartItems).ThenInclude(ci => ci.Product).FirstOrDefaultAsync(c => c.Id == cartId && c.CartStatus == Cart.Status.ACTIVE);
                if(cart is null)
                {
                    return Forbid("Cart unavailable");
                }
                cart.CompleteCart();
                var shipping = JsonConvert.DeserializeObject<Shipping>((string)TempData["checkout-shipping2"]);
                await _db.Payments.AddAsync(payment);
                await _db.Shippings.AddAsync(shipping);
                var order = new Order
                {
                    Cart = cart,
                    Shipping = shipping,
                    Payment = payment,
                };

                if (_signInManager.IsSignedIn(User))
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user is not null)
                    {
                        order.UserId= user.Id;
                        if (shipping.Save)
                        {
                            user.SavedShipping = shipping;
                        }
                        if (payment.Save)
                        {
                            user.SavedPayment = payment;
                        }
                    }
                }
                await _db.Orders.AddAsync(order);
                await _db.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("clearCart", cart.Id);
                TempData["checkout-order2"] = order.Id;
                HttpContext.Response.Cookies.Delete("cart_id");
                return RedirectToAction("Completed");
            }
            TempData.Keep();
            return View(payment);
        }

        public async Task<IActionResult> Completed(int orderId)
        {
            if (!TempData.ContainsKey("checkout-order2"))
            {
                return BadRequest();
            }
            var orderId= (int)TempData["checkout-order2"];
            var order= await _db.Orders.Include(o=>o.Cart).ThenInclude(c=>c.CartItems).ThenInclude(ci=>ci.Product).FirstAsync(o=>o.Id==orderId);
            return View(order);
        }
    }
}
