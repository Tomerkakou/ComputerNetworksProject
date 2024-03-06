using Microsoft.AspNetCore.SignalR;

namespace ComputerNetworksProject.Hubs
{
    public class ProductsHub:Hub
    {
        private readonly ILogger<ProductsHub> _logger;
        public ProductsHub(ILogger<ProductsHub> logger)
        {
            _logger = logger;
        }
        public override Task OnConnectedAsync()
        {
            _logger.LogInformation(Clients.Caller.ToString());
            return base.OnConnectedAsync();
        }
        public async Task ProductChanged(int productId,int stock)
        {
            await Clients.All.SendAsync("productNewAvailableStock",productId, stock);
        }
    }
}
