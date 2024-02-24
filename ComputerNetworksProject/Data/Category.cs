using System.ComponentModel.DataAnnotations;

namespace ComputerNetworksProject.Data
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Category() { 

        }

        public Category(string name)
        {
            Name = name;
        }
    }
}
