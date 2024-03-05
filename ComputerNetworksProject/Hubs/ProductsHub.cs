using Microsoft.AspNetCore.SignalR;

namespace ComputerNetworksProject.Hubs
{
    public class ProductsHub:Hub
    {
        public async Task ProductChanged(int productId,int stock)
        {
            await Clients.All.SendAsync("productNewAvailableStock",productId, stock);
        }
    }
}
