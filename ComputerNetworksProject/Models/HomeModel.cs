using ComputerNetworksProject.Data;

namespace ComputerNetworksProject.Models
{
    public class HomeModel:LayoutModel
    {
        public IEnumerable<Product>? Products { get; set; }

    }
}
