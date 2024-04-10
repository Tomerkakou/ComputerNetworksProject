using System.ComponentModel.DataAnnotations;

namespace ComputerNetworksProject.Data
{
    public class Category
    {
        [Key]
        [StringLength(40,MinimumLength =1,ErrorMessage ="Category name cannot be empty!")]
        public string Name { get; set; }

    }
}
