using ComputerNetworksProject.Data;

namespace ComputerNetworksProject.Models
{
    public class CategoriesModel:LayoutModel
    {
        public IEnumerable<Category>? Categories { get; set; }
        public Category Input { get; set; }
    }
}
