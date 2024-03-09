
using ComputerNetworksProject.Constants;
using ComputerNetworksProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ComputerNetworksProject.Services
{
    public class CartFilter: IAsyncActionFilter,IAsyncPageFilter
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CartFilter> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;    
        public CartFilter(ApplicationDbContext db,ILogger<CartFilter> logger, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _db = db;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //needs to be copied to every identity page 
            User? user=null;
            Cart? userCart=null,finalCart=null;
            int cookieCartId;
            //return logged in user
            var userClaim = context.HttpContext.User;
            if (_signInManager.IsSignedIn(userClaim))
            {
                user = await _userManager.GetUserAsync(userClaim);
                if (user is not null)
                {
                    userCart = await _db.Carts.Include(c => c.CartItems).ThenInclude(c => c.Product).FirstOrDefaultAsync(c => c.UserId == user.Id && c.CartStatus == Cart.Status.ACTIVE);
                }
            }
            if (int.TryParse(context.HttpContext.Request.Cookies["cart_id"], out cookieCartId))
            {
                var cookieCart = await _db.Carts.Include(c => c.CartItems).ThenInclude(c => c.Product).FirstOrDefaultAsync(c => c.Id == cookieCartId && c.CartStatus == Cart.Status.ACTIVE);
                if(cookieCart is null)
                {
                    context.HttpContext.Response.Cookies.Delete("cart_id");
                }
                if (user is null && cookieCart is not null && cookieCart.UserId is null)
                {
                    //no user and cart not belong to any user
                    var cookieOptions = new CookieOptions
                    {
                        Expires = Constant.CookieOffset,
                    };
                    context.HttpContext.Response.Cookies.Append("cart_id", cookieCartId.ToString(), cookieOptions);
                    finalCart = cookieCart;
                }
                else if (user is not null && cookieCart is null && userCart is not null)
                {
                    //cookie cart is not active but usercart is active
                    context.HttpContext.Response.Cookies.Delete("cart_id");
                    finalCart = userCart;
                }
                else if (user is not null && cookieCart is not null && userCart is not null && cookieCart.Id == userCart.Id)
                {
                    //same cart
                    context.HttpContext.Response.Cookies.Delete("cart_id");
                    finalCart = userCart;
                }
                else if (user is not null && cookieCart is not null && userCart is not null && cookieCart.Id != userCart.Id)
                {
                    //TWO carts but not the same
                    if (cookieCart.UserId is null || cookieCart.UserId == user.Id)
                    {
                        //combine two carts if the cookie cart belong to that user or not belong to anyone
                        userCart.MergeCart(cookieCart);
                        _db.Carts.Remove(cookieCart);
                        await _db.SaveChangesAsync();
                        context.HttpContext.Response.Cookies.Delete("cart_id");
                        finalCart = userCart;
                    }
                }
                else if(user is not null && cookieCart is not null && userCart is null)
                {
                    //cart not belong to any user make the connection
                    context.HttpContext.Response.Cookies.Delete("cart_id");
                    cookieCart.UserId = user.Id;
                    await _db.SaveChangesAsync();
                    finalCart= cookieCart;
                }
            }
            else 
            {
                //no cookie cart id 
                finalCart = userCart;
            }
            Controller controller = context.Controller as Controller;

            if (finalCart is not null)
            {
                finalCart.LastUpdate=DateTime.Now;
                await _db.SaveChangesAsync();
                var deletedCartItem=finalCart.CartItems.Where(ci=>ci.Product.ProductStatus==Product.Status.DELETED).ToList();
                if(deletedCartItem.Count > 0) {
                    string combinedNames = string.Join(", ", deletedCartItem.Select(ci => ci.Product.Name));
                    controller.TempData["info"] = $"Products : {combinedNames} removed from your cart";
                    foreach(var item in deletedCartItem)
                    {
                        _db.CartItems.Remove(item);
                    }
                    await _db.SaveChangesAsync();
                    if(finalCart.GetItemsCount() == 0)
                    {  
                        _db.Carts.Remove(finalCart);
                        await _db.SaveChangesAsync();
                        context.HttpContext.Response.Cookies.Delete("cart_id");
                        finalCart = null;
                    }
                }
            }

            controller.ViewData["Cart"] = finalCart;

            var resultContext = await next();

        }

        public async Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            //no need
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            //needs to be copied to every identity page 
            User? user = null;
            Cart? userCart = null, finalCart = null;
            int cookieCartId;
            //return logged in user
            var userClaim = context.HttpContext.User;
            if (_signInManager.IsSignedIn(userClaim))
            {
                user = await _userManager.GetUserAsync(userClaim);
                if (user is not null)
                {
                    userCart = await _db.Carts.Include(c => c.CartItems).ThenInclude(c => c.Product).FirstOrDefaultAsync(c => c.UserId == user.Id && c.CartStatus == Cart.Status.ACTIVE);
                }
            }
            if (int.TryParse(context.HttpContext.Request.Cookies["cart_id"], out cookieCartId))
            {
                var cookieCart = await _db.Carts.Include(c => c.CartItems).ThenInclude(c => c.Product).FirstOrDefaultAsync(c => c.Id == cookieCartId && c.CartStatus == Cart.Status.ACTIVE);
                if (cookieCart is null)
                {
                    context.HttpContext.Response.Cookies.Delete("cart_id");
                }
                if (user is null && cookieCart is not null && cookieCart.UserId is null)
                {
                    //no user and cart not belong to any user
                    var cookieOptions = new CookieOptions
                    {
                        Expires = Constant.CookieOffset,
                    };
                    context.HttpContext.Response.Cookies.Append("cart_id", cookieCartId.ToString(), cookieOptions);
                    finalCart = cookieCart;
                }
                else if (user is not null && cookieCart is null && userCart is not null)
                {
                    //cookie cart is not active but usercart is active
                    context.HttpContext.Response.Cookies.Delete("cart_id");
                    finalCart = userCart;
                }
                else if (user is not null && cookieCart is not null && userCart is not null && cookieCart.Id == userCart.Id)
                {
                    //same cart
                    context.HttpContext.Response.Cookies.Delete("cart_id");
                    finalCart = userCart;
                }
                else if (user is not null && cookieCart is not null && userCart is not null && cookieCart.Id != userCart.Id)
                {
                    //TWO carts but not the same
                    if (cookieCart.UserId is null || cookieCart.UserId == user.Id)
                    {
                        //combine two carts if the cookie cart belong to that user or not belong to anyone
                        userCart.MergeCart(cookieCart);
                        _db.Carts.Remove(cookieCart);
                        await _db.SaveChangesAsync();
                        context.HttpContext.Response.Cookies.Delete("cart_id");
                        finalCart = userCart;
                    }
                }
                else if (user is not null && cookieCart is not null && userCart is null)
                {
                    //cart not belong to any user make the connection
                    context.HttpContext.Response.Cookies.Delete("cart_id");
                    cookieCart.UserId = user.Id;
                    await _db.SaveChangesAsync();
                    finalCart = cookieCart;
                }
            }
            else
            {
                //no cookie cart id 
                finalCart = userCart;
            }

            PageModel pagemodel = context.HandlerInstance as PageModel;

            if (finalCart is not null)
            {
                finalCart.LastUpdate = DateTime.Now;
                await _db.SaveChangesAsync();
                var deletedCartItem = finalCart.CartItems.Where(ci => ci.Product.ProductStatus == Product.Status.DELETED).ToList();
                if (deletedCartItem.Count > 0)
                {
                    //string combinedNames = string.Join(", ", deletedCartItem.Select(ci => ci.Product.Name));
                    //pagemodel.TempData["info"] = $"Products : {combinedNames} removed from your cart";
                    foreach (var item in deletedCartItem)
                    {
                        _db.CartItems.Remove(item);
                    }
                    await _db.SaveChangesAsync();
                    if (finalCart.GetItemsCount() == 0)
                    {
                        _db.Carts.Remove(finalCart);
                        await _db.SaveChangesAsync();
                        context.HttpContext.Response.Cookies.Delete("cart_id");
                        finalCart = null;
                    }
                }
            }

            pagemodel.ViewData["Cart"] = finalCart;

            await next();
        }
    }
}
