using ComputerNetworksProject.Data;

namespace ComputerNetworksProject.Models
{
    public class CategoriesModel
    {
        public IEnumerable<Category>? Categories { get; set; }
        public Category Input { get; set; }
    }
}
