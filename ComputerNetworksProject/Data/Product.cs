using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerNetworksProject.Data
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "Please enter valid float Number")]
        public float Price { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Please enter valid float Number")]
        public float? PriceDiscount { get; set; }

        [Required]
        [StringLength(20,MinimumLength =1)]
        public string Name { get; set; }

        [StringLength(60)]
        public string? Description { get; set; }

        [Range(1,5)]
        [DefaultValue(3)]
        public int Stars { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public byte[]? Img { get; set; }
        public string? ImgType { get; set; }

        public Product()
        {

        }
        public Product(string name,float price,Category category, string? description,float? priceDiscount):this()
        {
            Name = name;
            Price = price;
            PriceDiscount = priceDiscount;
            Description = description;
            Category = category;
            CategoryId = category.Id;
        }
    }
}
