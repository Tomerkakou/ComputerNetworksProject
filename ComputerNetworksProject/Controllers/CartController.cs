using ComputerNetworksProject.Constants;
using ComputerNetworksProject.Data;
using ComputerNetworksProject.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
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
            if(product.ProductStatus==Product.Status.DELETED)
            {
                return BadRequest();
            }
            Cart? cart = (Cart?)ViewData["Cart"];
            bool newCart = false;
            if(cart is null)
            {
                cart=new Cart();
                newCart = true;
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
                var cartItem=cart.AddProduct(product);
                await _db.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("productNewAvailableStock", productId, product.AvailableStock);
                
                if (newCart)
                {
                    if (_signInManager.IsSignedIn(User))
                    {
                        var user = await _userManager.GetUserAsync(User);
                        if (user is not null)
                        {
                            await _hub.Clients.Users(user.Id).SendAsync("newCart", cart.Id);
                        }
                    }
                    else
                    {
                        //reset cart expire
                        var cookieOptions = new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(Constant.CookieOffset),
                        };
                        HttpContext.Response.Cookies.Append("cart_id", cart.Id.ToString(), cookieOptions);
                    }
                }
                else
                {
                    await _hub.Clients.All.SendAsync("cartChanged", cartItem.ProductId,cartItem.Amount, cartItem.GetPrice(), cart.Id, cart.GetTotalPrice(), cart.GetItemsCount());
                }
                return Ok(new {cartId=cart.Id,cartCount=cart.GetItemsCount()});
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
            if (product is null)
            {
                return BadRequest("not valid product id");
            }
            if (product.ProductStatus == Product.Status.DELETED)
            {
                return BadRequest();
            }
            if (cart is null)
            {
                return StatusCode(403,"Cart has been removed due to long idle");
            }
            try
            {
                var cartItem = cart.DecreaseItemAmount((int)productId);
                await _db.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("productNewAvailableStock", productId, product.AvailableStock);
                await _hub.Clients.All.SendAsync("cartChanged", cartItem.ProductId,cartItem.Amount, cartItem.GetPrice(), cart.Id, cart.GetTotalPrice(), cart.GetItemsCount());
                if (!User.Identity.IsAuthenticated)
                {
                    //reset cart expire
                    var cookieOptions = new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(Constant.CookieOffset),
                    };
                    HttpContext.Response.Cookies.Append("cart_id", cart.Id.ToString(), cookieOptions);
                }
                return Ok();
            }
            catch(ArgumentNullException ex)
            {
                return BadRequest("NO such product in cart");
            }
        }

        public async Task<IActionResult> DeleteItem(int? productId)
        {
            Cart? cart = (Cart?)ViewData["Cart"];
            if (productId is null)
            {
                return BadRequest("No productId");
            }
            if(cart is null)
            {
                var msg = TempData["info"] is not null ? TempData["info"] : "Cart has been removed due to long idle";
                return StatusCode(403, msg);
            }
            try
            {
                var cartItem = cart.DeleteItem((int)productId);
                var product = cartItem.Product;
                _db.CartItems.Remove(cartItem);
                await _hub.Clients.All.SendAsync("productNewAvailableStock", productId, product.AvailableStock);
            }
            catch
            {
                //not do anything
            }
            finally
            {
                if (cart.GetItemsCount() == 0)
                {
                    //delete cart
                    _db.Carts.Remove(cart);
                    await _db.SaveChangesAsync();
                    await _hub.Clients.All.SendAsync("clearCart", cart.Id);
                    HttpContext.Response.Cookies.Delete("cart_id");
                }
                else
                {
                    // only remove one item
                    await _db.SaveChangesAsync();
                    await _hub.Clients.All.SendAsync("cartItemRemove", productId, cart.Id, cart.GetTotalPrice(), cart.GetItemsCount());
                    if (!User.Identity.IsAuthenticated)
                    {
                        //reset cart expire
                        var cookieOptions = new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(Constant.CookieOffset),
                        };
                        HttpContext.Response.Cookies.Append("cart_id", cart.Id.ToString(), cookieOptions);
                    }
                }
                
            }

            return Ok();    

        }

        public async Task<IActionResult> DeleteCart()
        {
            Cart? cart = (Cart?)ViewData["Cart"];
            if (cart is null)
            {
                return Ok();
            }
            var products = cart.ClearCart();
            //delete cart
            _db.Carts.Remove(cart);
            await _db.SaveChangesAsync();
            await _hub.Clients.All.SendAsync("clearCart", cart.Id);
            HttpContext.Response.Cookies.Delete("cart_id");
            foreach (var product in products)
            {
                await _hub.Clients.All.SendAsync("productNewAvailableStock", product.Id, product.AvailableStock);
            }
            return Ok();
        }

        public async Task<IActionResult> GetCartItem(int? productId,int? cartId)
        {
            if(cartId is null || productId is null)
            {
                return BadRequest("no productId or cartId");
            }
            var cartItem = await _db.CartItems.FindAsync(cartId, productId);
            if(cartItem is null)
            {
                return BadRequest("No matching caritem");
            }
            return PartialView("_CartItemPartial",cartItem);
        }

        public async Task<IActionResult> GetCart(int? cartId)
        {
            var cart = await _db.Carts.FindAsync(cartId);
            return PartialView("_CartPartial", cart);
        }

        public async Task<IActionResult> BuyNow(int productId)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == productId);
            if (product is null)
            {
                return BadRequest("not valid product id");
            }

            var cart = new Cart();
            cart.BuyNow = true;
            await _db.Carts.AddAsync(cart);
            
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user is not null)
                {
                    cart.UserId = user.Id;
                }
            }
            try
            {
                var cartItem = cart.AddProduct(product);
                await _db.SaveChangesAsync();
                await _hub.Clients.All.SendAsync("productNewAvailableStock", productId, product.AvailableStock);

                return RedirectToAction("Review","Checkout",new {cartId=cart.Id});
            }
            catch (ArgumentException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (DbUpdateException ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
