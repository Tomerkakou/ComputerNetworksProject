using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;

namespace ComputerNetworksProject.Data
{
    [Index(nameof(CartId), IsUnique = true)]
    public class Order
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("UserId")]
        public string? UserId { get; set; }

        [ForeignKey("PaymentId")]
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }

        [ForeignKey("ShippingId")]
        public int ShippingId { get; set; }
        public Shipping Shipping { get; set; }

        [ForeignKey("CartId")]
        public int CartId { get; set; }
        public Cart Cart { get; set; }
    }
}
