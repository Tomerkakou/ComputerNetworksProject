
using ComputerNetworksProject.Constants;
using ComputerNetworksProject.Data;
using ComputerNetworksProject.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ComputerNetworksProject.Services
{
    public class CartReleaseService : BackgroundService
    {
        private readonly ILogger<CartReleaseService> _logger;
        private readonly IServiceProvider _service;

        public CartReleaseService(ILogger<CartReleaseService> logger, IServiceProvider service)
        {
            _logger = logger;
            _service= service;
        }
   

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting CartReleaseService");
            while (!stoppingToken.IsCancellationRequested)
            {
                await ClearCarts();
                await Task.Delay(Constant.ClearCartDelay, stoppingToken);
            }
            _logger.LogInformation("Stopping CartReleaseService");
        }
        private async Task ClearCarts()
        {
            using (var scope = _service.CreateScope()) // this will use `IServiceScopeFactory` internally
            {
                var db = scope.ServiceProvider.GetService<ApplicationDbContext>();
                var hub = scope.ServiceProvider.GetService<IHubContext<ProductsHub>>();
                var min = DateTime.Now.AddMinutes(-Constant.CookieOffset);
                var carts = await db.Carts.Include(c => c.CartItems).ThenInclude(c => c.Product).Where(c => c.CartStatus == Cart.Status.ACTIVE).Where(c => c.LastUpdate < min).ToListAsync();
                foreach (var cart in carts)
                {
                    cart.ClearCart();
                    _logger.LogInformation("Clearing cart {}", cart.Id);
                    await hub.Clients.All.SendAsync("clearCart", cart.Id);
                    db.Carts.Remove(cart);
                }
                await db.SaveChangesAsync();
            }
        } 
    }
}
