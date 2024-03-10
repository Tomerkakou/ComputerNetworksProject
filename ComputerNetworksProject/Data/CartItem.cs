using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComputerNetworksProject.Data
{
    [PrimaryKey(nameof(CartId),nameof(ProductId))]
    public class CartItem
    {


        [DefaultValue(0)]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        public int Amount { get; set; }
  
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        [Required]
        public Product Product { get; set; }

        [ForeignKey("CartId")]
        [Required]
        public int CartId { get; set; }

        public CartItem() { }

        public float GetPrice()
        {
            if(Product.PriceDiscount is null)
            {
                return Product.Price * Amount;
            }
            return (float)(Product.PriceDiscount*Amount);
        }
    }
}
