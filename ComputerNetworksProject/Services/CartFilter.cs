using ComputerNetworksProject.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace ComputerNetworksProject.Services
{
    public class CartFilter: IAsyncActionFilter
    {
        private readonly ApplicationDbContext _db;
        public CartFilter(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //needs to be copied to every identity page 
            Controller controller = context.Controller as Controller;

            controller.ViewData["Cart"] = await _db.Carts.Include(c => c.CartItems).ThenInclude(c => c.Product).FirstAsync();

            var resultContext = await next();

        }
    }
}
